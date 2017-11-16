using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// This component stores camera data in a GameObject. It is meant to be added to PathNode objects.
/// </summary>
public class CameraDataNode : MonoBehaviour {
    /// <summary>
    /// Contains a reference to a camera for use in all forms of node GUI viewing
    /// </summary>
    [HideInInspector]
    static public GameObject GUIcam;

    /// <summary>
    /// The raw camera data to use when looking at this object.
    /// </summary>
    [System.Serializable]
    public class CameraData
    {
        /// <summary>
        /// How far away the camera should be from the target. Measured in meters.
        /// </summary>
        [Tooltip("How far away the camera should be from the target. Measured in meters.")]
        [Range(5, 50)] public float cameraDistance = 15;
        /// <summary>
        /// How much pitch (y-rotation) the camera rig should have. Measured in degrees.
        /// </summary>
        [Tooltip("How much pitch (y-rotation) the camera rig should have. Measured in degrees.")]
        [Range(-89, 89)] public float pitchOffset;
        /// <summary>
        /// How much yaw (x-rotation) the camera rig should have. Measured in degrees.
        /// </summary>
        [Tooltip("How much yaw (x-rotation) the camera rig should have. Measured in degrees.")]
        [Range(-80, 80)] public float yawOffset;
        /// <summary>
        /// The field of view of the camera (i.e. lens angle). Measured in degrees.
        /// </summary>
        [Tooltip("The field of view of the camera (i.e. lens angle). Measured in degrees.")]
        [Range(50, 120)] public float fov = 50;
        /// <summary>
        /// The ease multiplier to use. This affects the interpolation of all other properties.
        /// </summary>
        [Tooltip("The ease multiplier to use. This affects the interpolation of all other properties.")]
        [Range(0, 5)] public float easeMultiplier = 2;
        /// <summary>
        /// Performs a linear interpolation on all members of two CameraData objects.
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <param name="p">Percentage (0 - 1). 0 will yield data1; 1 will yield data2.</param>
        /// <returns></returns>
        static public CameraData Lerp( CameraData data1, CameraData data2, float p )
        {
            CameraData data3 = new CameraData();
            data3.cameraDistance = Mathf.Lerp(data1.cameraDistance, data2.cameraDistance, p);
            data3.yawOffset = Mathf.Lerp(data1.yawOffset, data2.yawOffset, p);
            data3.pitchOffset = Mathf.Lerp(data1.pitchOffset, data2.pitchOffset, p);
            data3.easeMultiplier = Mathf.Lerp(data1.easeMultiplier, data2.easeMultiplier, p);
            data3.fov = Mathf.Lerp(data1.fov, data2.fov, p);
            return data3;
        }

    }
    /// <summary>
    /// Stores camera data.
    /// </summary>
    public CameraData cameraData;
    /// <summary>
    /// Determines whether or not a camera window is drawn in the scene.
    /// </summary>
    [HideInInspector]
    public bool drawSceneViewer = true;

    #region gizmo rendering

    /// <summary>
    /// Draws any gizmos while rendering the scene view.  Projects a representation of the camera.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawIcon(GetCameraLocation(), "icon-camera", true);//draw a camera icon where the cam would be.
        Matrix4x4 temp = Gizmos.matrix;//In order to use frustrum drawing, some matrix translations are required.
        Gizmos.matrix = Matrix4x4.TRS(GetCameraLocation(), GetCameraRotation(), Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, cameraData.fov, cameraData.cameraDistance * 2, 0, 4/3);
        Gizmos.matrix = temp;//reset the gizmo location.
    }
    /// <summary>
    /// Returns the vector location in worldspace where the camera node will orient the camera.
    /// </summary>
    /// <returns>Camera location in world space.</returns>
    public Vector3 GetCameraLocation()
    {
        Quaternion rotation = GetCameraRotation();
        Vector3 camPos = new Vector3(0, 0, -cameraData.cameraDistance);
        camPos = rotation * camPos;//Applies pitch and yaw to the camera's position relative to the node
        camPos += transform.position;//Apply that relative position to world space
        return camPos;
    }
    /// <summary>
    /// Calculates and returns the Quaternion rotation the camera node will orient the camera to.
    /// </summary>
    /// <returns>Quaternion rotation for the camera.</returns>
    public Quaternion GetCameraRotation()
    {
        /* Camera yaw (movement around the y axis (along the horizon) is calculated in two parts.  The first part is the orientation of the path node itself, and then the additional yaw offset specified by the camera node.  Both need to be added together. */
        Quaternion worldRotation = GetComponent<PathNode>().GetRotationOnCurve(.5f);
        float yawAngle = worldRotation.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(cameraData.pitchOffset, yawAngle - cameraData.yawOffset, 0);

        return rotation;
    }

    /// <summary>
    /// Creates a new camera to use for GUI purposes if one doesn't exist.
    /// </summary>
    public static void CreateGUICam()
    {
        /*We need a separate camera to preview the node. If it hasn't already been done in this scene, we create a new camera, and name it separate from the main camera.  Now we can move this camera around to each node when we want to preview it.  The camera is hidden in the heirarchy to keep things clean, and deleted by other functions when no longer in use.*/
        if( GameObject.Find("NodePreviewCam") ) { CameraDataNode.GUIcam = GameObject.Find("NodePreviewCam"); } else
        {
            CameraDataNode.GUIcam = Instantiate(GameObject.FindGameObjectWithTag("MainCamera"));
            CameraDataNode.GUIcam.name = "NodePreviewCam";
            CameraDataNode.GUIcam.hideFlags = HideFlags.HideInHierarchy;
        }
    }

    #endregion
}

/// <summary>
/// A custom editor for the camera node that will draw handles for users to manipulate the camera in the scene view and draw a camera preview window in the scene view.  Adds new buttons to the inspector.
/// </summary>
[CustomEditor(typeof(CameraDataNode))]
public class CameraNodeSceneGUI : Editor
{
    /// <summary>
    /// Stores a reference to the node this editor is used for.
    /// </summary>
    CameraDataNode camNode;
    /// <summary>
    /// Stores the Rect component of the camera viewport window.
    /// </summary>
    private Rect camWindow = new Rect(Screen.width - 430, Screen.height - 360, 400, 300);//TODO: adjustable size scalar & aspect ratio?

    /// <summary>
    /// Called when the editor is created. Links the node the editor targets to an internal variable.
    /// </summary>
    private void OnEnable()
    {
        camNode = (CameraDataNode)target;//register this editor to the script that called it
    }
    /// <summary>
    /// Called to when the interface of the inspector is changed.  Adds 2 new buttons to activate viewing windows.
    /// </summary>
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();//Make sure we have the normal camera data node inspector elements.
        if(CameraDataNode.GUIcam) OrientGUICamera();//Aligns the camera to the node's specifications.  This needs to be done every time a change is made so that it's updated for the editor version of the viewing window
        if(GUILayout.Button(new GUIContent("View in Scene Window", "Draw a viewport window in the scene view that displays the camera node's camera view.  May take a second to respond.")) )
        {
            camNode.drawSceneViewer = !camNode.drawSceneViewer;//toggle the variable
            if( !camNode.drawSceneViewer )//if we're not drawing the scene-viewing window
            {
                if(!FindObjectOfType<CameraNodeWindow>())//if there isn't an editor window
                {
                    //clean up the GUI camera
                    DestroyImmediate(CameraDataNode.GUIcam);
                    CameraDataNode.GUIcam = null;
                }
            } 
        }
        if( GUILayout.Button(new GUIContent("View in Editor Window", "Opens a new editor window that displays the camera nodes' camera view.")) )
        {
            EditorWindow.GetWindow<CameraNodeWindow>();
        }
    }

    /// <summary>
    /// Custom functionality called every frame a GUI element is to be rendered in the scene.
    /// </summary>
    private void OnSceneGUI()
    {
        //If the draw camera has been toggled on we'll create a viewport window to see the camera.
        if( camNode.drawSceneViewer )
        {
            //Draw a Window GUI element that we'llr ender the camera in
            Handles.BeginGUI();//start a 2dGUI drawing session
            int id = 0;
            camWindow = GUI.Window(id, camWindow, DrawSceneViewerWindow, new GUIContent("Camera View", "A preview of what the camera will see when oriented to this camera node."));
            Handles.EndGUI();//End the 2dGUI session
        }        
    }
    /// <summary>
    /// A callback function that draws the contents of a custom GUI window.
    /// </summary>
    /// <param name="id">The ID # of the window being drawn</param>
    void DrawSceneViewerWindow(int id)
    {
        CameraDataNode.CreateGUICam();
        //draw the camera's view (offsets compensate for window name bar and border)
        Handles.DrawCamera(new Rect(0, 16, camWindow.width-1, camWindow.height - 17), CameraDataNode.GUIcam.GetComponent<Camera>(), DrawCameraMode.Normal);

        GUI.DragWindow();//allows the window to be dragged around the scene view.
    }
    /// <summary>
    /// Maves a GUIcam to the orientation suggested by the camera node.
    /// </summary>
    void OrientGUICamera()
    {
        //place this camera where the node would place it
        CameraDataNode.GUIcam.transform.position = camNode.GetCameraLocation();
        CameraDataNode.GUIcam.transform.rotation = camNode.GetCameraRotation();
        CameraDataNode.GUIcam.GetComponent<Camera>().fieldOfView = camNode.cameraData.fov;
    }
}

/// <summary>
/// A custom draggable, dockable editor window that will render a camera view as dictated by a camera node.
/// </summary>
public class CameraNodeWindow : EditorWindow
{
    /// <summary>
    /// Initialize the window
    /// </summary>
    [MenuItem("Window/Camera Node Viewer")]// Add menu to the Window menu
    static void Init()
    {
        //Make a new window and give it a name and tooltip
        CameraNodeWindow window = (CameraNodeWindow)EditorWindow.GetWindow(typeof(CameraNodeWindow));
        window.titleContent = new GUIContent("Camera Node Viewer", "Displays a render window of the selected camera node.");
        window.Show();//display the window
    }
    /// <summary>
    /// Called when the window finishes intializing.  Registers the camera.
    /// </summary>
    private void Awake()
    {
        if(!CameraDataNode.GUIcam ) CameraDataNode.CreateGUICam();
    }
    /// <summary>
    /// Called every frame to draw GUI elements.  Draws the camera view in the window.
    /// </summary>
    private void OnGUI()
    {
        if( !CameraDataNode.GUIcam ) CameraDataNode.CreateGUICam();
        Rect camRect = new Rect(0, 0, Screen.width, Screen.height);//use the size of the window, can be dynamically resized.
        Handles.DrawCamera(camRect, CameraDataNode.GUIcam.GetComponent<Camera>());
        Repaint();//manually repaints the window
    }
    /// <summary>
    /// Called when the window is closed.  Cleans up the GUI camera.
    /// </summary>
    private void OnDestroy()
    {
        if( CameraDataNode.GUIcam )
        {
            DestroyImmediate(CameraDataNode.GUIcam);//Destroy Immediate is for editor destruction, use instead of destroy
            CameraDataNode.GUIcam = null;
        }
    }
}
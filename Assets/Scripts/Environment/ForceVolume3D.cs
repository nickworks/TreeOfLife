using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// A trigger script for collision volumes that applies forces to objects that enter the volume. Requires some kind of 2D collider! For the volume to work, the collider component on the same object needs to be a trigger!
/// </summary>
public class ForceVolume3D : MonoBehaviour
{
    /// <summary>
    /// A pair of time values.
    /// </summary>
    [System.Serializable]
    public class TimeOnOff
    {
        /// <summary>
        /// How many seconds the force volume should be off.
        /// </summary>
        public float secondsOff = 1;
        /// <summary>
        /// How many seconds the force volume should be on.
        /// </summary>
        public float secondsOn = 1;
    }

    #region Variables

    /// <summary>
    /// Controls whether this volume will apply force to pawns or not.
    /// </summary>
    public bool turnedOn = true;
    /// <summary>
    /// How strong the force is in this particular volume 
    /// </summary>
    [Range(0, 100)] public float forceMult = 2f;
    /// <summary>
    /// Toggles whether or not to a ray in the scene view in the direction of this volume's force. 
    /// </summary>
    public bool drawEditorGizmo = false;
    /// <summary>
    /// The angle of the forces applied by this volume. 0 is directly upwards.
    /// </summary>
    [Range(-180, 180)] public float angleOfForce = 0f;
    /// <summary>
    /// What directional vector the force of this volume goes.  Should be normalized.
    /// </summary>
    private Vector3 forceVector = Vector3.up;
    /// <summary>
    /// Should this volume overwrite the other object's gravity, or simply add to it's velocity?
    /// </summary>
    public bool overwritesGravity = false;
    /// <summary>
    /// The timer controling toggling (used if isTimed = true)
    /// </summary>
    private float timer = 0f;
    /// <summary>
    /// When toggled off, should this object also disable the mesh on this object?
    /// </summary>
    public bool toggleMesh = true;
    /// <summary>
    /// The property keeps track of which burst we are currently on.
    /// </summary>
    private int burstIndex = 0;
    /// <summary>
    /// This property stores multiple bursts of wind.
    /// </summary>
    public TimeOnOff[] bursts;
    #endregion

    /// <summary>
    /// Called when the script starts.  Makes sure any collider on this volume is properly set to be a trigger.
    /// </summary>
    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    #region EditorAdjustments
    /// <summary>
    /// Called every time a value in the inspector is changed.  Updates force direction information.
    /// </summary>
    public void OnValidate()
    {
        //Create a rotation based on the angle specified in the editor, and then apply it to a vector.
        forceVector = Quaternion.AngleAxis(-angleOfForce, Vector3.forward) * Vector3.up;
    }

    /// <summary>
    /// Called when rendering in scene view.  Draws a line in the direction of force.
    /// </summary>
    private void OnDrawGizmos()
    {
        if( drawEditorGizmo ) {
            float scalar = Mathf.Min(transform.localScale.x, transform.localScale.y) * .5f;
            Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(forceVector), scalar, EventType.Repaint);
            Matrix4x4 camToWorld = SceneView.currentDrawingSceneView.camera.cameraToWorldMatrix;
            Handles.DrawWireDisc(transform.position, camToWorld * Vector3.forward, scalar / 4);
            Handles.DrawSolidArc(transform.position, camToWorld * Vector3.forward, camToWorld * Vector3.up, forceMult * 3.6f, scalar / 4);
        }
    }

    #endregion

    /// <summary>
    /// Every tick, if the "is timed" option is checked, the game cycles the timer and adjusts the volume accordingly
    /// </summary>
    private void Update()
    {
        if(bursts.Length > 0)
        {
            timer -= Time.deltaTime;
            if( timer < 0f )
            {
                if(turnedOn)
                {
                    timer = bursts[burstIndex].secondsOff;
                    turnedOn = false;
                } else
                {
                    timer = bursts[burstIndex].secondsOn;
                    turnedOn = true;
                    burstIndex++;
                    if (burstIndex >= bursts.Length) burstIndex = 0;
                }
                // print(timer);
                // toggle visibility of the volume if the option to do so is enabled
                if( toggleMesh ) GetComponent<MeshRenderer>().enabled = turnedOn;
            }
        }
    }

    #region Trigger Events

    /// <summary>
    /// Called when things enter the volume. Sets gravity if volume overwrites gravity.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter( Collider collision )
    {
        if( turnedOn )
        {
            if( overwritesGravity )
            {
                //only apply this to the player pawns
                if( collision.tag == "Player" )
                {
                    collision.GetComponent<Player.PlayerController>().SetGravity(forceVector, forceMult);
                }
            }
        }
    }
    /// <summary>
    /// Called when things exit the volume.  Resets gravity to game normal.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit( Collider collision )
    {
        if( turnedOn )
        {
            if( overwritesGravity )
            {
                if( collision.tag == "Player" )
                {
                    collision.GetComponent<Player.PlayerController>().SetGravity();
                }
            }
        }
    }
    /// <summary>
    /// Called every frame something is in this volume.  Applies force to a player.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay( Collider collision )
    {
        if( turnedOn )
        {
            if( !overwritesGravity )
            {
                if( collision.tag == "Player" )
                {
                    collision.GetComponent<Player.PlayerController>().ApplyForce(forceMult, forceVector);
                }
            }
        }
    }

    #endregion

}

/// <summary>
/// A custom editor that allows for timer options to be hidden if the volume is not timed.
/// </summary>
[CustomEditor(typeof(ForceVolume3D))]
public class ForceVolume3DEditor : Editor
{
    override public void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        /*
        ForceVolume3D volume = (ForceVolume3D)target;

        if (GUI.changed)
        {
            EditorUtility.SetDirty(volume);
            Undo.RegisterCompleteObjectUndo(volume, "Stuff changed");
            volume.OnValidate();
        }
        /**/
    }

}
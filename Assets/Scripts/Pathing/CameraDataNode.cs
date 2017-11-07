using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This component stores camera data in a GameObject. It is meant to be added to PathNode objects.
/// </summary>
public class CameraDataNode : MonoBehaviour {

    /// <summary>
    /// The raw camera data to use when looking at this object.
    /// </summary>
    [System.Serializable]
    public class CameraData
    {
        /// <summary>
        /// How far away the camera should be from the target. Measured in meters.
        /// </summary>
        [Range(5, 50)] public float cameraDistance = 15;
        /// <summary>
        /// How much pitch (y-rotation) the camera rig should have. Measured in degrees.
        /// </summary>
        [Range(-89, 89)] public float pitchOffset;
        /// <summary>
        /// How much yaw (x-rotation) the camera rig should have. Measured in degrees.
        /// </summary>
        [Range(-80, 80)] public float yawOffset;
        /// <summary>
        /// The field of view of the camera (i.e. lens angle). Measured in degrees.
        /// </summary>
        [Range(50, 120)] public float fov = 50;
        /// <summary>
        /// The ease multiplier to use. This affects the interpolation of all other properties.
        /// </summary>
        [Range(0, 5)] public float easeMultiplier = 2;
        /// <summary>
        /// Performs a linear interpolation on all members of two CameraData objects.
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <param name="p">Percentage (0 - 1). 0 will yield data1; 1 will yield data2.</param>
        /// <returns></returns>
        static public CameraData Lerp(CameraData data1, CameraData data2, float p)
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
    /// Stores camera data
    /// </summary>
    public CameraData cameraData;

    // TODO: build an in-editor preview of the camera
}

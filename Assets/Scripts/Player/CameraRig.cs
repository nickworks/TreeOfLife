using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script causes a camera to follow and align with a specified target. Add this script to the parent of a Camera GameObject.
/// </summary>
public class CameraRig : MonoBehaviour {

    [System.Serializable]
    public class CameraData
    {
        [Range(10, 30)] public float distance = 15;
        [Range(-45, 89)] public float pitch;
        [Range(-80, 80)] public float yaw;
        [Range(0, 10)] public float easing = 2;
        [Range(10, 50)] public float influenceRadius = 10;
    }

    /// <summary>
    /// The transform to ease towards.
    /// </summary>
    public Transform target;

    /// <summary>
    /// A scalar that affects how much easing the camera has.
    /// </summary>
    [Range(.5f, 10)] public float easing = 1;
    [Range(-45, 89)] public float pitch = 0;

    private Transform cam;
    private AlignWithPath trackData;

    /// <summary>
    /// This sets a reference to the Camera component.
    /// </summary>
	void Start () {
        cam = GetComponentInChildren<Camera>().transform;
        trackData = target.GetComponent<AlignWithPath>();
    }
    
    /// <summary>
    /// This causes the camera rig to follow, align with, and look at the specified target;
    /// </summary>
	void LateUpdate () {

        // TODO: we might want to store camera settings within PathNode objects... (zoom amount, easing, etc).

        if (!target) return;

        float yaw = target.rotation.eulerAngles.y;
        float distance = cam.localPosition.z;
        if (trackData)
        {
            CameraData data = trackData.currentNode.cameraData;
            distance = -data.distance;
            pitch = data.pitch;
            easing = data.easing;
            yaw += data.yaw;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(pitch, yaw, 0), Time.deltaTime * easing);
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * easing);

        // TODO: We will probably want to Lerp / Slerp the rotation and somehow limit it so it doesn't
        // always rotate to point directly at its target. This works fine for the player, but not for cutscenes.
        cam.rotation = Quaternion.LookRotation(target.position - cam.position, Vector3.up);
        cam.localPosition = Vector3.Lerp(cam.localPosition, new Vector3(0, 0, distance), Time.deltaTime * easing);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script causes a camera to follow and align with a specified target. Add this script to the parent of a Camera GameObject.
/// </summary>
public class CameraRig : MonoBehaviour {
    
    /// <summary>
    /// The transform to ease towards.
    /// </summary>
    public Transform target;

    /// <summary>
    /// A scalar that affects how much easing the camera has.
    /// </summary>
    [Range(.5f, 10)] public float easing = 1;
    /// <summary>
    /// A reference to a child GameObject with a camera component on it.
    /// </summary>
    private Camera cam;
    /// <summary>
    /// A reference to our target's AlignWithPathNode
    /// </summary>
    private AlignWithPath trackDataSrc;

    /// <summary>
    /// The camera's pitch. Measured in degrees.
    /// </summary>
    private float pitch = 0;
    /// <summary>
    /// The camera's yaw offset. Measured in degrees.
    /// </summary>
    private float yaw = 0;
    /// <summary>
    /// The camera's distance from its target. Measured in meters.
    /// </summary>
    private float distance = 10;
    /// <summary>
    /// The camera's field of view. Measured in degrees.
    /// </summary>
    private float fov = 90;

    /// <summary>
    /// This sets a reference to the Camera component.
    /// </summary>
	void Start () {
        cam = GetComponentInChildren<Camera>();
        if(target) trackDataSrc = target.GetComponent<AlignWithPath>();
    }
    
    /// <summary>
    /// This causes the camera rig to follow, align with, and look at the specified target;
    /// </summary>
	void LateUpdate () {

        if (!target) return;

        GetTrackData();

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(pitch, yaw, 0), Time.deltaTime * easing);
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * easing);

        // TODO: We will probably want to Lerp / Slerp the rotation and somehow limit it so it doesn't
        // always rotate to point directly at its target. This works fine for the player, but not for cutscenes.
        cam.transform.rotation = Quaternion.LookRotation(target.position - cam.transform.position, Vector3.up);
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(0, 0, -distance), Time.deltaTime * easing);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, Time.deltaTime * easing);
	}
    /// <summary>
    /// If the target has an AlignWithPath component, this method extracts 
    /// interpolated camera data from the target's current PathNode.
    /// </summary>
    private void GetTrackData()
    {
        yaw = target.rotation.eulerAngles.y;
        if (!trackDataSrc) return;
        if (!trackDataSrc.isActiveAndEnabled) return;

        CameraDataNode.CameraData data = trackDataSrc.GetCameraData();
        if (data == null) return;

        // Maybe we should ease these values instead of directly applying them...
        distance = data.cameraDistance;
        pitch = data.pitchOffset;
        easing = data.easeMultiplier;
        yaw -= data.yawOffset;
        fov = data.fov;
    }
}

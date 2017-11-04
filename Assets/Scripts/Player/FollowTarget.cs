using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script causes a camera to follow and align with a specified target. Add this script to the parent of a Camera GameObject.
/// </summary>
public class FollowTarget : MonoBehaviour {

    /// <summary>
    /// The transform to ease towards.
    /// </summary>
    public Transform target;
    /// <summary>
    /// A scalar that affects how much easing the camera has.
    /// </summary>
    [Range(.5f, 10)] public float easing = 1;

    private Transform cam;

    /// <summary>
    /// This sets a reference to the Camera component.
    /// </summary>
	void Start () {
        cam = GetComponentInChildren<Camera>().transform;
	}
	
    /// <summary>
    /// This causes the camera rig to follow, align with, and look at the specified target;
    /// </summary>
	void LateUpdate () {

        // TODO: we might want to store camera settings within PathNode objects... (zoom amount, easing, etc).

        if (!target) return;

        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * easing);
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * easing);

        // TODO: We will probably want to Lerp / Slerp the rotation and somehow limit it so it doesn't
        // always rotate to point directly at its target. This works fine for the player, but not for cutscenes.
        cam.rotation = Quaternion.LookRotation(target.position - cam.position, Vector3.up);

	}
}

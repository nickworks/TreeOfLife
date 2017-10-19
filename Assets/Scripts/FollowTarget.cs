using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	void Start () {
        cam = GetComponentInChildren<Camera>().transform;
	}
	
	void Update () {

        // TODO: We will probably want to Lerp / Slerp the rotation and somehow limit it so it doesn't
        // always rotate to point directly at its target. This works fine for the player, but not for cutscenes.
        cam.rotation = Quaternion.LookRotation(target.position - cam.position, Vector3.up);

        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * easing);
	}
}

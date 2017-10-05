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

	void Start () {
		
	}
	
	void Update () {

        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * easing);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    /// <summary>
    /// The transform to ease towards.
    /// </summary>
    public Transform target;
    /// <summary>
    /// A scalar that affects how much easing the camera has.
    /// </summary>
   

    void Start()
    {

    }

    void Update()
    {

        transform.position = target.position;
    }
}

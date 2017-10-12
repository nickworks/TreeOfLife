using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracker : MonoBehaviour {
    public Transform playerTransform;
    public Vector3 camaraOffset;
    public Vector3 playerTransformConverter;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        playerTransformConverter = playerTransform.position;
        camaraOffset.z = -5;
        camaraOffset.x = playerTransformConverter.x;

        camaraOffset.y = playerTransformConverter.y;

        transform.position = camaraOffset;
    }
}

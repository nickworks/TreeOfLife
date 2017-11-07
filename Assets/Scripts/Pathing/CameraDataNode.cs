using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDataNode : MonoBehaviour {

    [System.Serializable]
    public class CameraData
    {
        [Range(10, 30)] public float distance = 15;
        [Range(-45, 89)] public float pitch;
        [Range(-80, 80)] public float yaw;
        [Range(0, 10)] public float easing = 2;
        [Range(10, 50)] public float influenceRadius = 10;

        static public CameraData Lerp(CameraData data1, CameraData data2, float p)
        {
            CameraData data3 = new CameraData();
            data3.distance = Mathf.Lerp(data1.distance, data2.distance, p);
            data3.yaw = Mathf.Lerp(data1.yaw, data2.yaw, p);
            data3.pitch = Mathf.Lerp(data1.pitch, data2.pitch, p);
            data3.easing = Mathf.Lerp(data1.easing, data2.easing, p);
            return data3;
        }
    }

    public CameraData cameraData;

    public void GetCameraData (float p) {
		// -1 to 0 : interpolate from left to this
        //  0 to 1 : interpolate from this to right

        // need to correspond curveIn and curveOut to percent values...


        // curveIn to transform.position (.5 on curve)
        // transform.position (.5 on curve) to curveOut
        // curveOut to right.curveIn


	}
}

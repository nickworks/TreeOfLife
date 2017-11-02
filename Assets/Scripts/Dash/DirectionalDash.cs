using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// FIX ME - add commenting
public class DirectionalDash : MonoBehaviour {

    /// <summary>
	/// used to hold the rotation of the object.
	/// </summary>
	static public Vector3 rot = new Vector3();
    /// <summary>
    /// used to orient the object to the mouse.
    /// </summary>
    Vector3 pt = new Vector3();

    /// <summary>
    /// stores up direction to be used by the player class
    /// </summary>
    void Update () {
        FaceMouse();
		rot = this.transform.up; 
		//print (rot);
    }


    /// <summary>
    /// used to get the arrow to point to the mouse
    /// </summary>	
    void FaceMouse()
    {
        Plane plane = new Plane(Vector3.back, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        if(plane.Raycast(ray, out distance))
        {
             pt = ray.GetPoint(distance);
            //print(pt);
        }      
       pt.x = pt.x - transform.position.x;
       pt.y = pt.y - transform.position.y;
        transform.up = pt;        
    }
}

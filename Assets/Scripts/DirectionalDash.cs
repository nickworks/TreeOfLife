using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalDash : MonoBehaviour {


	static public Vector3 rot = new Vector3();
	/// <summary>
	/// used to hold the rotation of the object.
	/// </summary>
    Vector3 pt = new Vector3();
	/// <summary>
	/// used to orient the object to the mouse.
	/// </summary>
    
    void Start () {
		
    }
	

	void Update () {
        FaceMouse();

		rot = this.transform.up; // stores up direction to be used by the player class
	

		//print (rot);


    }



	// used to get the arrow to point to the mouse
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

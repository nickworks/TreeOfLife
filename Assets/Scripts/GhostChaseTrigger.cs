using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostChaseTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            print("Chase!");
            GetComponentInParent<Ghost>().inRange = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //print("Chase!");
            GetComponentInParent<Ghost>().targetPos = collision.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            print("Stop chase");
            GetComponentInParent<Ghost>().inRange = false;
        }
        
    }
}

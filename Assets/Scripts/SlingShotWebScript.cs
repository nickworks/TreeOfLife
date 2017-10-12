using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShotWebScript : MonoBehaviour {

    /// <summary>
    /// A variable to hold a player controller script reference
    /// </summary>
    PlayerController pc;
    /// <summary>
    /// An integer to hold the direction the player is boosted
    /// </summary>
    public int direction;
	// Use this for initialization
	void Start () {
        //This gets the player controller reference
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// A collision event listener
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If the collision has the player tab
        if(collision.gameObject.tag == "Player")
        {
            //Sets the players propulsion to the direction variable of this object
            pc.propulsionDirection = direction;
        }
    }
}

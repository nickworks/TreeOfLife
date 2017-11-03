using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// This class is used to control the fly hives that spit out bugs to chase the player
/// </summary>
public class FlyController : MonoBehaviour {

#region Variable Region
    /// <summary>
    /// A float variable to hold the distance between the player and the fly
    /// </summary>
    public float distance;
    /// <summary>
    /// The flies aggro range
    /// </summary>
    public float aggroRange;

    /// <summary>
    /// A public boolean used to tell the bug to go alert the hive when the player gets within a certain position
    /// </summary>
    public bool alerrtHive;
    /// <summary>
    /// A public bool to change the bugs behavior
    /// </summary>
    public bool rage = false;

    /// <summary>
    /// A float variable that is used to control the bugs speed
    /// </summary>
    public float speed;
    /// <summary>
    /// A public transform to hold the targets position
    /// </summary>
    public Transform target;
    /// <summary>
    /// A vector 3 that is used to move the bug around
    /// </summary>
    Vector3 flyMover = new Vector3();
    /// <summary>
    /// A public boolean used to control wiether or not the bug is stopped
    /// </summary>
    public bool isStopped;
#endregion
   
	
	// Update is called once per frame
	void Update ()
    {
        //As long as the bug this script is attached to is not stopped
        if (!isStopped)
        {
            //speed += velocity * Time.deltaTime;
            //This uses the vector 3 distance function to set distance equal to our targets transform subtracted from the bugs transform
            distance = Vector3.Distance(transform.position, target.position);
            //If the alert hive variable is true
            if (alerrtHive == true)
            {
                //The bug goes to alert the hive
                AlertHive();
            }

            //If the player is within the aggro range we chase the player
            if (rage == false && distance < aggroRange)
            {
                //The bug goes to alert the hive
                alerrtHive = true;

            }
            //Otherwise if the bug is angry and has already alerted the hive it chases the player
            if (rage == true && distance < aggroRange)
            {
                //Calls the chase player function
                ChasePlayer();
            }
            //The transform position gets the flymover vector added to it
            transform.position += flyMover;
        }
		
	}



	
    /// <summary>
    /// A private method to handle the logic for chasing the player
    /// </summary>
    private void ChasePlayer()
    {
        
        //If the players  x position is greater than the bugs position we chase them to the right
        if (target.position.x > transform.position.x)
        {
            //We add speed multiplied by deltaTime to the flymover.x
			flyMover.x += speed * Time.deltaTime;         
        }
        else if (target.position.x < transform.position.x)//IF the players x position is less than the bugs position we chase them to the left
        {
            //We subtract speed multiplied by deltaTime to the flymover.x
            flyMover.x -= speed * Time.deltaTime;
        }
        //If the players  y position is greater than the bugs position we chase them up
        if (target.position.y >= transform.position.y) 
		{
            //We add speed multiplied by deltaTime to the flymover.y
            flyMover.y += speed * Time.deltaTime;		
		}	
		else if (target.position.y <= transform.position.y) //If the players  y position is less than the bugs position we chase them down
        {
            //We subtract speed multiplied by deltaTime to the flymover.y
            flyMover.y -= speed * Time.deltaTime;			
		}	

    }
    /// <summary>
    /// A private method to handle the logic for alerting the hive
    /// </summary>
    private void AlertHive()
    {
       //We subtract flymover.x by speed multiplied by deltatime to have the bug run to the hive to the right of it
		flyMover.x -= speed * Time.deltaTime;
    }

    /// <summary>
    ///A 2d trigger enter method used to handle collision data
    /// </summary>
    /// <param name="collision"> A collision event</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If the bugs hit a stickyweb their velocity is set to zero
        if (collision.gameObject.tag == "StickyWeb")
        {
            //The bug is stopped if it hits the web
            isStopped = true;          
        }
    }
    /// <summary>
    ///A 2d trigger stay method used to handle collision data
    /// </summary>
    /// <param name="collision"> A collision event</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        //If the bugs hit a stickyweb their velocity is set to zero
        if (collision.gameObject.tag == "StickyWeb")
        {
            //The bug is stopped if it stays in the web
            isStopped = true;
        }
    }
}

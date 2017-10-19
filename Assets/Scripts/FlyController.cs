using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class is named FlyController but it should probably be called leaderbug controller or something else because the bugs don't fly yet
/// </summary>
public class FlyController : MonoBehaviour {

    /// <summary>
    /// A variable to hold Rigidbody references
    /// </summary>
    Rigidbody2D rb;
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
    /// A public float to hold velocity that speed is multiplied by
    /// </summary>
    public float velocity;


    /// <summary>
    /// A start method used for initialization
    /// </summary>
    void Start ()
    {
        //Gets the rigid body reference
        rb = GetComponent<Rigidbody2D>();
        //target = GameObject.Find("Player").GetComponent<Transform>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //This uses the vector 3 distance function to set distance equal to our targets transform subtracted from the bugs transform
         distance = Vector3.Distance(transform.position, target.position);
        //If the alert hive variable is true
        if(alerrtHive == true)
        {
            //The bug goes to alert the hive
            AlertHive();
        }

        //If the player is within the aggro range we chase the player
        if(rage == false && distance< aggroRange)
        {
            //The bug goes to alert the hive
            alerrtHive = true;
            
        }
        //Otherwise if the bug is angry and has already alerted the hive it chases the player
        if(rage == true && distance < aggroRange)
        {
            //Calls the chase player function
            ChasePlayer();
        }
	}

    /// <summary>
    /// A private method to handle the logic for chasing the player
    /// </summary>
    private void ChasePlayer()
    {
        //If the players position is greater than the bugs position we chase them to the right
        if (target.position.x > transform.position.x)
        {
            //Moves the bug to the right by using speed multiplied by velocity
            rb.AddForce(transform.right * speed * velocity);
        }
        else if (target.position.x < transform.position.x)//IF the players position is less than the bugs position we chase them to the left
        {
            //Moves the bug to the left by using speed multiplied by velocity
            rb.AddForce(transform.right * -speed * velocity);
        }
    }
    /// <summary>
    /// A private method to handle the logic for alerting the hive
    /// </summary>
    private void AlertHive()
    {
        //Moves the bug to the left by using speed multiplied by velocity
        rb.AddForce(transform.right * -speed * velocity);
    }
}

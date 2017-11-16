using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A public class for the smaller follower bugs
/// </summary>
public class FollowerBugController : MonoBehaviour {



    #region Variable Region

    
    /// <summary>
    /// A float variable to hold the distance between the player and the fly
    /// </summary>
    public float distance;
    /// <summary>
    /// The flies aggro range
    /// </summary>
    public float attentionRange;

    /// <summary>
    /// A float variable that is used to control the bugs speed
    /// </summary>
    public float speed;

    /// <summary>
    /// A boolean used to control wiether or not the bugs are stopped
    /// </summary>
    public bool isStopped;

    /// <summary>
    /// A public transform to hold the targets position
    /// </summary>
    public Transform leaderTarget;

    
    /// <summary>
    /// Sets the bugs state
    /// </summary>
    public int state =  2;
    /// <summary>
    /// A variable used to control movement
    /// </summary>
    Vector3 flyMover = new Vector3();


#endregion

    // Use this for initialization
    void Start ()
    {
        //If state is one
      if(state == 1)
        {
            //Gets the leaderbugs transform
            leaderTarget = GameObject.Find("LeaderBug").GetComponent<Transform>();
        }
      else if(state == 2)//If state is two
        {
            //Gets the players transform
            leaderTarget = GameObject.Find("Player").GetComponent<Transform>();
        }
      
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        
        //If is stopped is false
        if (!isStopped)
        {
            //This uses the vector 3 distance function to set distance equal to our targets transform subtracted from the bugs transform
            distance = Vector3.Distance(transform.position, leaderTarget.position);
            //If the distance is less than the attention range
            if (distance < attentionRange)
            {
                //the bugs chase the leader
                ChaseLeader();
            }
            //The flymover vector is added to the transform position
            transform.position += flyMover;
        }
           
    }

    /// <summary>
    /// A private void to handle the bugs chasing the leader
    /// </summary>
    private void ChaseLeader()
    {
        
        //If the leaderbugs position is greater than the bugs position we chase them to the right
        if (leaderTarget.position.x > transform.position.x)
        {
           //Flymover.x gets speed multiplied by deltatime added to it
            flyMover.x += speed * Time.deltaTime;

        }
        else if (leaderTarget.position.x < transform.position.x)//IF the leaderbugs position is less than the bugs position we chase them to the left
        {
            //Flymover.x gets speed multiplied by deltatime subtracted from it
            flyMover.x -= speed * Time.deltaTime;
        }
        //If leaderTargets y position is less than the players y position we chase them down
        if (leaderTarget.position.y < transform.position.y)
        {
            //Flymover.y gets speed multiplied by deltatime subtracted from it
            flyMover.y -= speed * Time.deltaTime;
        }
        else if (leaderTarget.position.y > transform.position.y) // If leaderTargets y position is greater than the bugs poisition we chase them up
        {
            //Flymover.y gets speed multiplied by deltatime added to it
            flyMover.y += speed * Time.deltaTime;
        }
    }



    /// <summary>
    ///A 2d trigger enter method used to handle collision data
    /// </summary>
    /// <param name="collision"> A collision event</param>
    private void OnTriggerEnter(Collider collision)
    {
        //If the bugs hit a stickyweb their velocity is set to zero
        if(collision.gameObject.tag == "StickyWeb")
        {

            //The bug is stopped if it hits the web
            isStopped = true;
            
        }
    }
    /// <summary>
    ///A 2d trigger stay method used to handle collision data
    /// </summary>
    /// <param name="collision"> A collision event</param>
    private void OnTriggerStay(Collider collision)
    {
        //If the bugs hit a stickyweb their velocity is set to zero
        if (collision.gameObject.tag == "StickyWeb")
        {

            //The bug is stopped as long as it is in the web
            isStopped = true;
        }
    }
}

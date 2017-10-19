using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A public class for the smaller follower bugs
/// </summary>
public class FollowerBugController : MonoBehaviour {

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
    public float attentionRange;

    /// <summary>
    /// A float variable that is used to control the bugs speed
    /// </summary>
    public float speed;


    /// <summary>
    /// A public transform to hold the targets position
    /// </summary>
    public Transform leaderTarget;


    /// <summary>
    /// A vector 3 that is used to move the bug around
    /// </summary>
    Vector3 bugMover = new Vector3();

    // Use this for initialization
    void Start ()
    {
        //Gets the reference
        rb = GetComponent<Rigidbody2D>();
        //Gets the leaderbugs transform
        leaderTarget = GameObject.Find("LeaderBug").GetComponent<Transform>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //This uses the vector 3 distance function to set distance equal to our targets transform subtracted from the bugs transform
        distance = Vector3.Distance(transform.position, leaderTarget.position);
        //If the distance is less than the attention range
        if(distance < attentionRange)
        {
            //the bugs chase the leader
            ChaseLeader();
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
           //Uses the rigidbody to move the follower bugs
            rb.AddForce(transform.right * speed);

        }
        else if (leaderTarget.position.x < transform.position.x)//IF the leaderbugs position is less than the bugs position we chase them to the left
        {
            //Uses the rigidbody to move the follower bugs
            rb.AddForce(transform.right * -speed);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class FlyBehavior : BehaviorNPC {

    #region Variable Region
    /// <summary>
    /// A integer used to control what type of FlyBehavior is used
    /// If 1 the fly is a leader and other flies should follow the leader
    /// If 2 the fly is a follower
    /// </summary>
    public int state = 2;
    /// <summary>
    /// A float variable to hold the distance between the player and the fly
    /// </summary>
    public float distance;
    /// <summary>
    /// The flies aggro range
    /// </summary>
    public float aggroRange;
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


    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        //TODO: Expand Fly AI Logic
        //TODO: Change Fly AI logic to differentiate between follower and leader
        //TODO: SPawn Flies from Hives
        if (!isStopped)
        {
            
            distance = Vector3.Distance(target.position, transform.position);

            ChasePlayer();
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

        //If the players  y position is greater than the bugs position we chase them up
        if (target.position.z >= transform.position.z)
        {
            //We add speed multiplied by deltaTime to the flymover.y
            flyMover.z += speed * Time.deltaTime;
        }
        else if (target.position.z <= transform.position.z) //If the players  y position is less than the bugs position we chase them down
        {
            //We subtract speed multiplied by deltaTime to the flymover.y
            flyMover.z -= speed * Time.deltaTime;
        }
    }

    public override void FindsPlayer(PlayerController player)
    {
        
    }

    public override void LosesPlayer()
    {
        
    }

    public override void PlayerNearby(PlayerController player)
    {
        
    }

    public override void IsStopped()
    {
        isStopped = true;
        print("Working?");
    }

}

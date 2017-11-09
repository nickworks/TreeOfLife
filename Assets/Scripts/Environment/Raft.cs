﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>FIXME - controls A raft object
/// Is the main script that controls all raft objects in the game.
/// </summary>
public class Raft : MonoBehaviour
{
    /// <summary>
    /// Controls whether the raft is affected by gravity, this is turned true when colliding with water.
    /// </summary>
    private bool isInWater = false;
    /// <summary>FIXME gravity should be consistent with player
    /// The direction of gravity applied when not colliding with water.
    /// </summary>
    private Vector3 gravity = Vector3.down;
    /// <summary>
    /// Holds a reference to the player object.
    /// </summary>
    private Player.PlayerController pawn;
    //FIXME summary comment
    private Transform pawn2;
    /// <summary>
    /// Which layers we want the raft to collide with.
    /// </summary>
    public LayerMask waterMask;

    //FIXME Summary Comments for all of these
    // gets the boxCollider2D bounds of the object and stores it in a variable
    private Bounds bounds;
    // stores the left minimum position of the object in a Vector2
    private Vector3 leftMin;
    // stores the right minimum position of the object in a Vector2
    private Vector3 rightMin;
    // stores the right minimum position of the object in a Vector2
    private Vector3 centerMin;
    // stores the right minimum position of the object in a Vector2
    private float halfH;
    //FIXME Summary comments
    void Start()
    {
        bounds = GetComponent<BoxCollider>().bounds;
        halfH = Mathf.Abs(bounds.center.y - bounds.min.y);
    }

    /// <summary>FIXME this is all treated as in-line in a tooltip, should be one complete paragraph
    /// updates gravity every frame if not colliding with water
    /// updates horizontal velocity every frame if the player "isAttached" to the raft
    /// </summary>
    void LateUpdate()
    {
        CalculateBounds();
        // is true when colliding with water, else false
        if (isInWater == false)
        {
            //FIXME - add a debug toggle or remove the print messages
            Debug.DrawRay(centerMin, Vector3.down);
            if (Physics.Raycast(centerMin, Vector3.down, halfH, waterMask))
            {
                
                print("iscolliding");
                isInWater = true;
            }
            else
            {
                transform.position += gravity * Time.deltaTime;
            }

        }

        // is true when the player is attached to the raft
        if (pawn != null)
        {
            // applies velocity to the raft
            transform.position += DetermineHorizontalMovement();
            //transform.position += transform.TransformVector(DetermineHorizontalMovement()) * Time.deltaTime; FIXME remove old code
        }

    }

    /// <summary>
    /// Calculates the positions for Raycasts to be sent from the raft.
    /// </summary>
    private void CalculateBounds()
    {
        bounds = this.GetComponent<BoxCollider>().bounds;
        leftMin = new Vector3(bounds.min.x, bounds.center.y, bounds.center.z);
        rightMin = new Vector3(bounds.max.x, bounds.center.y, bounds.center.z);
        print(bounds.min);//FIXME remove debug prints or add to a "debug" toggle in inspector
        print(bounds.max);
        centerMin = bounds.center;
    }
    /// <summary>FIXME - treat as one complete paragraph, not multiple lines. Move comments about methodology to the function itself.
    /// determines the rafts horizontal movement if it is touching water
    /// the raft casts rays from its left minimum y position and right minimum y position
    /// The raycasts make it so that a raft cannot move off of water once colliding
    /// they can however move back onto water if they hit the edge and one raycast is no longer colliding
    /// </summary>
    /// <returns>How much velocity to add to the raft.</returns>
    private Vector3 DetermineHorizontalMovement()
    {

        Vector3 raftWorldVelocity = new Vector3(pawn.worldSpace.x, 0, pawn.worldSpace.z);
        float playerX = pawn.velocity.x;

        Vector3 velocity = raftWorldVelocity;
        /// <summary>
        /// casts a ray downwards
        /// if the ray does not collide with water and the player is moving left
        /// stop the rafts horizontal movement
        /// </summary>FIXME these should be block comments /* */, not summaries.  Summary comments are for class/method headers.
        if (!Physics.Raycast(leftMin, Vector3.down, 1, waterMask))
        {
            Debug.DrawRay(leftMin, Vector3.down, Color.green, 5);
            if (pawn.velocity.x > 0)
            {
                print("stop");
                velocity.x = 0;
                velocity.z = 0;
            }
        }

        /// <summary>
        /// casts a ray downwards
        /// if the ray does not collide with water and the player is moving right
        /// stop the rafts horizontal movement
        /// </summary>FIXME these should be block comments /* */, not summaries.  Summary comments are for class/method headers.
        if( !Physics.Raycast(rightMin, Vector3.down, 1, waterMask))
        {
            Debug.DrawRay(rightMin, Vector3.down, Color.green, 5);
            if (pawn.velocity.x < 0)
            {
                print("stop");
                velocity.x = 0;
                velocity.z = 0;
            }
        }

        return velocity;
    }

    /// <summary>
    /// detects collision with objects that have the "Water" tag
    /// when collision occurs, downward gravity is stopped on the raft
    /// </summary>
    /// <param name="other"></param> the object the raft IS colliding with
    /**FIXME: clean this up
    void OnTriggerEnter(Collider other)
    {
        // sets isInWater to true if the raft is colliding with an object with "Water" Layer
        int mask = (1 << other.gameObject.layer);
        print("hello");
        if ((mask & waterMask.value) > 0)
        {
            isInWater = true;
        }
        
    }
    **/
    /// <summary>
    /// This method is called to attach the raft to a player.
    /// </summary>
    /// <param name="player">The PlayerController to attach to.</param>
    public void Attach(Player.PlayerController player)
    {
        pawn = player;
        pawn2 = player.GetComponent<Transform>();
        transform.rotation = pawn2.rotation;
    }
    /// <summary>
    /// This method detaches the raft from a PlayerController.
    /// </summary>
    public void Detach()
    {
        pawn = null;//FIXME: Does Pawn2 also need to be null?
    }
}

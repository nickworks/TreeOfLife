using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
namespace Player
{

    /// <summary>
    /// A public class to handle the players climbing state
    /// </summary>
    public class PlayerStateClimbing : PlayerState
    {

#region Variable Region
        /// <summary>
        /// A private float to handle the players climbspeed
        /// </summary>
        private float climbSpeed = 15;
        /// <summary>
        /// A private float to slow the player down in the volume
        /// </summary>
        private float slowDown = .8f;
        /// <summary>
        /// An int that stores what kind of climbing volume the player is in.
        /// </summary>
        private string climbingVolumeID;

        private bool doingStuff;
#endregion
        /// <summary>
        /// The class constructor. It's purpose is to set the ID of the climbing volume.
        /// </summary>
        /// <param name="ID">Identifies the climbing volume</param>
        public PlayerStateClimbing(string ID)
        {
            climbingVolumeID = ID;
        }

        /// <summary>
        /// This is called on state start, to immediately decelerate the player.
        /// </summary>
        public void Start()
        {
            doingStuff = true;
            player.velocity.y = player.velocity.y * 0.05f;
        }

        /// <summary>
        /// This method is called by the PlayerController every tick.
        /// </summary>
        /// <param name="player">The state-machine driven PlayerController object that called this method.</param>
        /// <returns>The new state that the PlayerController should switch to. If null, then remain in this state.</returns>
        override public PlayerState Update(PlayerController player)
        {
            //Updates the players state
            base.Update(player);
            
            //Appies gravity at a value of zero so the player does not fall
            ApplyGravity(0f);
            //Handles the players input
            HandleInput();
            //Does collisions
            DoCollisions();
            /*
            while (!doingStuff)
            {
                return this;
            }*/
            return null;
        }

        /// <summary>
        /// This method is called by the PlayerController when this state begins.
        /// </summary>
        /// <param name="player">The state-machine driven PlayerController object that called this method.</param>
        public override void OnEnter(PlayerController player) { }
        /// <summary>
        /// This method is called by the PlayerController when this state ends.
        /// </summary>
        /// <param name="player">The state-machine driven PlayerController object that called this method.</param>
        public override void OnExit(PlayerController player) { }

        /// <summary>
        /// This method uses input to manipulate this object's physics.
        /// </summary>
        private void HandleInput()
        {
            //Sets the players horizontal and vertical movement equal to a horizontal and vertical axis variable
            float axisH = Input.GetAxisRaw("Horizontal");
            float axisV = Input.GetAxisRaw("Vertical");

            // Velocity in the x-axis is slowed by 90% if the climbing volume is a rope
            switch (climbingVolumeID)
            {
                case "StickyWeb":
                    player.velocity.x += axisH * climbSpeed * Time.deltaTime;
                    if (player.velocity.x > climbSpeed * 0.5f) player.velocity.x = climbSpeed * 0.5f; // Speed cap
                    break;
                case "Rope":
                    player.velocity.x += axisH * climbSpeed * Time.deltaTime * .1f;
                    if (player.velocity.x > climbSpeed * 0.05f) player.velocity.x = climbSpeed * 0.05f; // Speed cap
                    break;
            }

            player.velocity.y += axisV * climbSpeed * Time.deltaTime;
            if (player.velocity.y > climbSpeed * 0.5f) player.velocity.y = climbSpeed * 0.5f; // Speed cap
            
            //Drag
            if (axisH == 0) player.velocity.x *= slowDown;
            if (axisV == 0) player.velocity.y *= slowDown;           

            //If the player presses the jump key
            if (Input.GetButton("Jump"))
            {
                player.velocity = new Vector3(0, player.velocity.y * 0.3f, player.velocity.z);      // 'Normalize' the velocity
                player.velocity += new Vector3(axisH * impulseJump * 0.2f, impulseJump * 0.8f, 0);  // add Jump Impulse
            }
        }
    }
}
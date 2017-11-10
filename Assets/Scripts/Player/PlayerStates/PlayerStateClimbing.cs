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
        private float climbSpeed = 30;
        /// <summary>
        /// A private float to handle the players jumping momentum
        /// </summary>
        private float jumpPulse = 850;
        /// <summary>
        /// A private float to slow the player down in the web
        /// </summary>
        private float slowDown = .55f;
        /// <summary>
        /// A private float to slow the player down when they jump
        /// </summary>
        private float jumpSlowDown = .75f;

#endregion

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
            //Hanldes the players input
            HandleInput();
            //Does collisions
            DoCollisions();

            return this;
        }

        /// <summary>
        /// This method is called by the PlayerController when this state begins.
        /// </summary>
        /// <param name="player">The state-machine driven PlayerController object that called this method.</param>
        override public void OnEnter(PlayerController player) {
            

        }
        /// <summary>
        /// This method is called by the PlayerController when this state ends.
        /// </summary>
        /// <param name="player">The state-machine driven PlayerController object that called this method.</param>
        override public void OnExit(PlayerController player) {
            //Slows the player down when they exit
            player.velocity.y *= jumpSlowDown;
        }
        /// <summary>
        /// This method uses input to manipulate this object's physics.
        /// </summary>
        private void HandleInput()
        {
            //Sets the players horizontal and vertical movement equal to a horizontal and vertical axis variable
            float axisH = Input.GetAxisRaw("Horizontal");
            float axisV = Input.GetAxisRaw("Vertical");
            //If the player presses the jump key
            if (Input.GetButtonDown("Jump"))
            {
                //We remove upward momentum resulting from climbing in the web
                player.velocity.y = 0;
                //then we add the jump pulse to their velocity
                player.velocity.y += jumpPulse * Time.deltaTime;
                //We add the players horizontal velocity
                player.velocity.x += axisH * climbSpeed * Time.deltaTime;
                
                
                
            }
            //If the input is horizontal or vertical
            if(Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                //We add the players climbspeed to their velocity
                player.velocity.x += axisH * climbSpeed * Time.deltaTime;
                player.velocity.y += axisV * climbSpeed * Time.deltaTime;
            }
            else
            {
                //Otherwise we multiply the velocity by the slowdown variable
                player.velocity.x *= slowDown;
                player.velocity.y *= slowDown;
            }
            
            
        }

    }
}
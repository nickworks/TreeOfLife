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
        /// A bool that checks what kind of climbing volume the player is in, and adjusts the physics accordingly
        /// </summary>
        private bool isWeb;

#endregion
        /// <summary>
        /// The class constructor. It's purpose is to figure out what climbing volume the player is in.
        /// </summary>
        /// <param name="isItWeb">If its not, then it's rope.</param>
        public PlayerStateClimbing(bool isItWeb)
        {
            isWeb = isItWeb;
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
            //player.velocity.y *= jumpSlowDown;
        }
        /// <summary>
        /// This method uses input to manipulate this object's physics.
        /// </summary>
        private void HandleInput()
        {
            //Sets the players horizontal and vertical movement equal to a horizontal and vertical axis variable
            float axisH = Input.GetAxisRaw("Horizontal");
            float axisV = Input.GetAxisRaw("Vertical");
            
            //If the input is horizontal or vertical
            if(Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                //We add the players climbspeed to their velocity based on climbable volume
                if (isWeb)
                {
                    player.velocity.x += axisH * climbSpeed * Time.deltaTime;
                }
                else
                {
                    player.velocity.x += axisH * climbSpeed * Time.deltaTime * .2f;
                }
                player.velocity.y += axisV * climbSpeed * Time.deltaTime;

                //Cap the climb speed
                if (player.velocity.y > climbSpeed * 0.5f) player.velocity.y = climbSpeed * 0.5f;
                if (player.velocity.x > climbSpeed * 0.1f) player.velocity.x = climbSpeed * 0.1f;
            }
            else
            {
                //Otherwise we multiply the velocity by the slowdown variable
                player.velocity.x *= slowDown;
                player.velocity.y *= slowDown;
            }

            //If the player presses the jump key
            if (Input.GetButtonDown("Jump"))
            {
                //We remove momentum resulting from climbing
                player.velocity.y = player.velocity.y * 0.3f;
                player.velocity.x = 0;

                //We set the players horizontal velocity
                player.velocity.x += axisH * impulseJump * 0.2f;

                //Then we set the jump pulse to their velocity
                player.velocity.y += impulseJump * 0.8f;
            }
        }
    }
}
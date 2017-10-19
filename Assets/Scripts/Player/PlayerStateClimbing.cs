using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
namespace Player
{

    
    public class PlayerStateClimbing : PlayerState
    {

        private float climbSpeed = 10;
        /// <summary>
        /// This method is called by the PlayerController every tick.
        /// </summary>
        /// <param name="player">The state-machine driven PlayerController object that called this method.</param>
        /// <returns>The new state that the PlayerController should switch to. If null, then remain in this state.</returns>
        override public PlayerState Update(PlayerController player)
        {
            base.Update(player);
            
            bool jumpActive = Jump(player.jumpVelocity);
            ApplyGravity(0f);
            HandleInput();
            DoCollisions();

            return null;
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

             
        }
        /// <summary>
        /// This method uses input to manipulate this object's physics.
        /// </summary>
        private void HandleInput()
        {
            float axisH = Input.GetAxisRaw("Horizontal");
            float axisV = Input.GetAxisRaw("Vertical");
            if (Input.GetButton("Jump"))
            {
                
                player.velocity.y += 200* Time.deltaTime;
                player.isClimbing = false;
                player.velocity.x += axisH * climbSpeed * Time.deltaTime;
                
                
            }
            player.velocity.x += axisH * climbSpeed * Time.deltaTime;
            player.velocity.y += axisV * climbSpeed * Time.deltaTime;
            
        }

    }
}
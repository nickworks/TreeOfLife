using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerStateRegular : PlayerState
    {

        /// <summary>
        /// This method is called by the PlayerController every tick.
        /// </summary>
        /// <param name="player">The state-machine driven PlayerController object that called this method.</param>
        /// <returns>The new state that the PlayerController should switch to. If null, then remain in this state.</returns>
        override public PlayerState Update(PlayerController player)
        {
            base.Update(player);

            bool jumpActive = Jump(player.jumpVelocity);
            
            ApplyGravity(jumpActive ? 1 : 2f);
            HandleInput();
            DoCollisions();

            return this;
        }
        /// <summary>
        /// This method is called by the PlayerController when this state begins.
        /// </summary>
        /// <param name="player">The state-machine driven PlayerController object that called this method.</param>
        override public void OnEnter(PlayerController player) { }
        /// <summary>
        /// This method is called by the PlayerController when this state ends.
        /// </summary>
        /// <param name="player">The state-machine driven PlayerController object that called this method.</param>
        override public void OnExit(PlayerController player) { }
        /// <summary>
        /// This method uses input to manipulate this object's physics.
        /// </summary>
        private void HandleInput()
        {
            float axisH = Input.GetAxisRaw("Horizontal");
            if (Input.GetButton("Jump"))
            {
                Jump(player.jumpVelocity);
            }
            if (axisH == 0)
            {
                DecelerateX(player.walkAcceleration);
            }
            else
            {
                bool movingLeft = (player.velocity.x <= 0);
                bool acceleratingLeft = (axisH <= 0);
                // if the player pushes the opposite direction from how they're moving, the player turns around quicker!
                float scaleAcceleration = (movingLeft != acceleratingLeft) ? player.turnAroundMultiplier : 1;

                AccelerateX(axisH * player.walkAcceleration * scaleAcceleration);
            }
        }

        
    }
}

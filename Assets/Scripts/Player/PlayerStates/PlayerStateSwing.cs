using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerStateSwing : PlayerState
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
            //ApplyGravity(jumpActive ? 1 : 2f);
            HandleInput();
            //DoCollisions();
            return null;
        }
        /// <summary>
        /// This method is called by the PlayerController when this state begins.
        /// </summary>
        /// <param name="player">The state-machine driven PlayerController object that called this method.</param>
        override public void OnEnter(PlayerController player)
        {
            

        }
        /// <summary>
        /// This method is called by the PlayerController when this state ends.
        /// </summary>
        /// <param name="player">The state-machine driven PlayerController object that called this method.</param>
        override public void OnExit(PlayerController player)
        {

        }
        /// <summary>
        /// This method uses input to manipulate this object's physics.
        /// </summary>
        private void HandleInput()
        {
            Transform pt = player.rigidBody.transform;
            float axisH = Input.GetAxisRaw("Horizontal");
            Vector2 dir = pt.position - player.ropeTarget.position;
            dir = new Vector2(-dir.y, dir.x);
            dir.Normalize();
            player.rigidBody.AddForce(dir * axisH * player.swingStrength);
        }

    }
}

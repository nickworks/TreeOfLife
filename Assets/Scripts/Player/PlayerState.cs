using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public abstract class PlayerState
    {

        public PlayerController player;
        /// <summary>
        /// This method is called by the PlayerController every tick.
        /// </summary>
        /// <param name="player">The state-machine driven PlayerController object that called this method.</param>
        /// <returns>The new state that the PlayerController should switch to. If null, then remain in this state.</returns>
        public virtual PlayerState Update(PlayerController player)
        {
            this.player = player;
            return null;
        }
        /// <summary>
        /// This method is called by the PlayerController when this state begins.
        /// </summary>
        /// <param name="player">The state-machine driven PlayerController object that called this method.</param>
        public abstract void OnEnter(PlayerController player);
        /// <summary>
        /// This method is called by the PlayerController when this state ends.
        /// </summary>
        /// <param name="player">The state-machine driven PlayerController object that called this method.</param>
        public abstract void OnExit(PlayerController player);

        /// <summary>
        /// This boolean tracks whether or not the player has a jump active. It allows us to have variable jump heights.
        /// </summary>
        protected bool isJumping = false;
        /// <summary>
        /// This boolean stores whether or not the player is currently on the ground.
        /// </summary>
        protected bool isGrounded = false;

        /// <summary>
        /// This method decelerates the horizontal speed of the object.
        /// </summary>
        /// <param name="amount">How much decelerating-force to apply.</param>
        protected void DecelerateX(float amount)
        {
            if (!player) return;
            // slow down the player
            if (player.velocity.x > 0) // moving right...
            {
                AccelerateX(-amount);
                if (player.velocity.x < 0) player.velocity.x = 0;
            }
            if (player.velocity.x < 0) // moving left...
            {
                AccelerateX(amount);
                if (player.velocity.x > 0) player.velocity.x = 0;
            }
        }
        /// <summary>
        /// This method accelerates the horizontal speed of the object.
        /// </summary>
        /// <param name="amount">The value of horizontal acceleration.</param>
        protected void AccelerateX(float amount)
        {
            if (!player) return;
            player.velocity.x += amount * Time.deltaTime;
        }
        /// <summary>
        /// This method checks to see if the player should jump. It also tracks whether or not the player has a jump active.
        /// </summary>
        /// <param name="velocityAtTakeoff"></param>
        /// <returns></returns>
        protected bool Jump(float velocityAtTakeoff)
        {
            // jump
            if (player.velocity.y < 0 || !Input.GetButton("Jump")) isJumping = false;

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                player.velocity.y = velocityAtTakeoff;
                isJumping = true;
            }
            return (Input.GetButton("Jump") && isJumping);
        }
        /// <summary>
        /// Apply the player.gravity to player.velocity. 
        /// </summary>
        /// <param name="scale">How much to scale the gravity before applying it. (1 = no scale)</param>
        protected void ApplyGravity(float scale)
        {
            // gravity
            player.velocity += player.gravityDir * player.gravityTemporary * Time.deltaTime * scale;
        }
        /// <summary>
        /// Perform collision detection by calling the PawnAABB's collision detection methods.
        /// The results of collision detection are then applied.
        /// </summary>
        protected void DoCollisions()
        {
            // clamp to max speed
            if (Mathf.Abs(player.velocity.x) > player.maxSpeed)//TODO: better way to handle volume acceleration?
            {
                player.velocity.x = Mathf.Sign(player.velocity.x) * player.maxSpeed;
            }

            PawnAABB.CollisionResults results = player.pawn.Move(player.velocity * Time.deltaTime);
            if (results.hitTop || results.hitBottom) player.velocity.y = 0;
            if (results.hitLeft || results.hitRight) player.velocity.x = 0;
            isGrounded = results.hitBottom || results.ascendSlope;
            player.transform.position += results.distance;
        }
    }
}

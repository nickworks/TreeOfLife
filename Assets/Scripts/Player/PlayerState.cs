using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState {

    public PlayerController player;
    public virtual PlayerState Update(PlayerController player)
    {
        this.player = player;
        return null;
    }
    public abstract void OnEnter(PlayerController player);
    public abstract void OnExit(PlayerController player);
    
    /// <summary>
    /// This boolean allows us to have variable jump heights.
    /// </summary>
    protected bool isJumping = false;
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
    protected void ApplyGravity(float scale)
    {
        // gravity
        player.velocity.y -= player.gravity * Time.deltaTime * scale;
    }
    /// <summary>
    /// Perform collision detection by calling the PawnAABB's collision detection methods.
    /// The results of collision detection are then applied.
    /// </summary>
    protected void DoCollisions()
    {
        // clamp to max speed
        if (Mathf.Abs(player.velocity.x) > player.maxSpeed)
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

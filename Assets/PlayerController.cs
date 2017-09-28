using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float jumpTime = 0.75f;
    public float jumpHeight = 3;

    public float acceleration = 10;

    float gravity;
    float jumpImpulse;

    bool isGrounded = false;
    bool isJumping = false;
    
    Vector3 velocity = new Vector3();
    PawnAABB pawn;
	
	void Start () {
        pawn = GetComponent<PawnAABB>();

        DeriveJumpValues();
	}
    void DeriveJumpValues()
    {
        gravity = (jumpHeight * 2) / (jumpTime * jumpTime);
        jumpImpulse = gravity * jumpTime;
    }
	
	void Update ()
    {
        HandleInput();

        // Do the move:
        PawnAABB.CollisionResults results = pawn.Move(velocity * Time.deltaTime);
        if (results.hitTop || results.hitBottom) velocity.y = 0;
        if (results.hitLeft || results.hitRight) velocity.x = 0;

        isGrounded = results.hitBottom;

        transform.position += results.distance;
    }

    private void HandleInput()
    {
        GravityAndJumping();

        // sideways movement
        float axisH = Input.GetAxisRaw("Horizontal");

        if (axisH == 0)
        {
            DecelerateX(acceleration);
        }
        else
        {
            AccelerateX(axisH * acceleration);
        }
    }

    private void GravityAndJumping()
    {
        float gravityScale = 2;

        // jump
        if (velocity.y < 0) isJumping = false;
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = jumpImpulse;
            isJumping = true;
            gravityScale = 0;
        }
        if (Input.GetButton("Jump"))
        {
            if (isJumping) gravityScale = 1;
        }
        else
        {
            isJumping = false;
        }

        // gravity
        velocity.y -= gravity * Time.deltaTime * gravityScale;
    }

    private void DecelerateX(float amount)
    {
        // slow down the player
        if (velocity.x > 0) // moving right...
        {
            AccelerateX(-amount);
            if (velocity.x < 0) velocity.x = 0;
        }
        if (velocity.x < 0) // moving left...
        {
            AccelerateX(amount);
            if (velocity.x > 0) velocity.x = 0;
        }
    }

    private void AccelerateX(float amount)
    {
        velocity.x += amount * Time.deltaTime;
    }
}

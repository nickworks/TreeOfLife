using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component turns a GameObject into a controllable avatar.
/// </summary>
[RequireComponent(typeof(PawnAABB))]
public class PlayerController : MonoBehaviour {
    
    /// <summary>
    /// A transform to hold the spawnpoints transform
    /// </summary>
    public Transform spawnTransform;
    /// <summary>
    /// A variable used to control when the spawnpoint moves
    /// </summary>
    public bool moveSpawn;
   
    

    /// <summary>
    /// A boolean for if the player is grabbing, this is activated by pressing up
    /// </summary>
    public bool grabbing;
    /// <summary>
    /// A boolean for if the player is dropping, this is activated by pressing down
    /// </summary>
    public bool dropping;


    /// <summary>
    /// The amount of time, in seconds, that it should take the player to reach the peak of their jump arc.
    /// </summary>
    /// 
    public float jumpTime = 0.75f;
    /// <summary>
    /// The height, in meters, of the player's jump arc.
    /// </summary>
    public float jumpHeight = 3;
    /// <summary>
    /// The horizontal acceleration to use when the player moves left or right.
    /// </summary>
    public float walkAcceleration = 10;
    /// <summary>
    /// The acceleration to use for gravity. This will be calculated from the jumpTime and jumpHeight fields. changed to public
    /// </summary>
    public float gravity;
    /// <summary>
    /// The takeoff speed to use as vertical velocity for the player's jump. This will be calculated from jumpTime and jumpHeight fields.
    /// </summary>
    public float jumpVelocity;
    /// <summary>
    /// Whether or not this PlayerController is on the ground. Set to public
    /// </summary>
    public bool isGrounded = false;
    /// <summary>
    /// Whether or not the player is currently jumping.Set to public
    /// </summary>
    public bool isJumping = false;
    /// <summary>
    /// The velocity of the player. This is used each frame for Euler physics integration. This is set to public to interact with other components
    /// </summary>
    public Vector3 velocity = new Vector3();
    /// <summary>
    /// A reference to the PawnAABB component on this object.
    /// </summary>
    private PawnAABB pawn;
    /// <summary>
    /// This initializes this component.
    /// </summary>
    void Start()
    {
        pawn = GetComponent<PawnAABB>();
        spawnTransform = GameObject.Find("SpawnPoint").GetComponent<Transform>();
        DeriveJumpValues();
    }
    /// <summary>
    /// This is called automatically when the values change in the inspector.
    /// </summary>
    void OnValidate()
    {
        DeriveJumpValues();
    }
    /// <summary>
    /// This method calculates the gravity and jumpVelocity to use for jumping.
    /// </summary>
    public void DeriveJumpValues()
    {
        gravity = (jumpHeight * 2) / (jumpTime * jumpTime);
        jumpVelocity = gravity * jumpTime;
    }
    /// <summary>
    /// This method is called each frame. 
    /// </summary>
    void Update()
    {
        HandleInput();
        DoCollisions();     
            
        
    }
    /// <summary>
    /// Perform collision detection by calling the PawnAABB's collision detection methods.
    /// The results of collision detection are then applied.
    /// </summary>
    private void DoCollisions()
    {
        PawnAABB.CollisionResults results = pawn.Move(velocity * Time.deltaTime);
        if (results.hitTop || results.hitBottom) velocity.y = 0;
        if (results.hitLeft || results.hitRight) velocity.x = 0;
        isGrounded = results.hitBottom || results.ascendSlope;
        transform.position += results.distance;
    }
    /// <summary>
    /// This method uses input to manipulate this object's physics. Set this to public so it can be called from other methods
    /// </summary>
    public void HandleInput()
    {
        GravityAndJumping();


        //Calls the spawning method so that if the player hits R they respawn
        Spawning();
        //Gets the players raw Horizontal Axis
        float axisH = Input.GetAxisRaw("Horizontal");
        //Gets the players raw Vertical axis
        float axisV = Input.GetAxisRaw("Vertical");
        //If input GetButtonDown is grab
        if (Input.GetButton("Grab"))
        {
            //then grabbing is true;
            grabbing = true;
        }//If the InputGetButtonUP is grab
        else if (Input.GetButtonUp("Grab"))
        {
            //then grabbing is false
            grabbing = false;
        }//If the inputGetButtonDown is Drop
        if (Input.GetButtonDown("Drop"))
        {
            //then grabbing is false
            grabbing = false;
        }


        if (axisH == 0)
        {
            DecelerateX(walkAcceleration);
        }
        else
        {
            bool movingLeft = (velocity.x <= 0);
            bool acceleratingLeft = (axisH <= 0);
            float scaleAcceleration = (movingLeft != acceleratingLeft) ? 5 : 1; // if the player pushes the opposite direction from how they're moving, the player turns around quicker!

            AccelerateX(axisH * walkAcceleration * scaleAcceleration);
        }

    }


    /// <summary>
    /// A private void to control spawning for the player
    /// </summary>
    private void Spawning()
    {
        //If the Input GetButtonDown is respawn
        if (Input.GetButtonDown("Respawn"))
        { //The transform position is set to the spawnTransform position
            transform.position = spawnTransform.position;
        }
    }

    /// <summary>
    /// This method calculates the vertical physics of this object.
    /// </summary>
    private void GravityAndJumping()
    {
        float gravityScale = 2;

        // jump
        if (velocity.y < 0) isJumping = false;
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = jumpVelocity;
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

   

    /// <summary>
    /// This method decelerates the horizontal speed of the object.
    /// </summary>
    /// <param name="amount">The scalar value of horizontal acceleration. This should be a positive number.</param>
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
    /// <summary>
    /// This method accelerates the horizontal speed of the object.
    /// </summary>
    /// <param name="amount">The value of horizontal acceleration.</param>
    private void AccelerateX(float amount)
    {
        velocity.x += amount * Time.deltaTime;
    }


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component turns a GameObject into a controllable avatar.
/// </summary>
[RequireComponent(typeof(PawnAABB))]
public class PlayerController : MonoBehaviour {

    /// <summary>
    /// The amount of time, in seconds, that it should take the player to reach the peak of their jump arc.
    /// </summary>
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
    /// The acceleration to use for gravity. This will be calculated from the jumpTime and jumpHeight fields.
    /// </summary>
    private float gravity;
    /// <summary>
    /// The takeoff speed to use as vertical velocity for the player's jump. This will be calculated from jumpTime and jumpHeight fields.
    /// </summary>
    private float jumpVelocity;
    /// <summary>
    /// Whether or not this PlayerController is on the ground.
    /// </summary>
    private bool isGrounded = false;
    /// <summary>
    /// The strength of the swing.
    /// </summary>
    public float swingStrength = 5;
    /// <summary>
    /// Whether or not the player is currently jumping.
    /// </summary>
    private bool isJumping = false;
    /// <summary>
    /// The velocity of the player. This is used each frame for Euler physics integration.
    /// </summary>
    private Vector3 velocity = new Vector3();
    /// <summary>
    /// A reference to the PawnAABB component on this object.
    /// </summary>
    private PawnAABB pawn;
    /// <summary>
    /// Reference to the rigidbody2D.
    /// </summary>
    private Rigidbody2D _rigidbody;
    /// <summary>
    /// The reference to the transform object of the rope swinging origin thing.
    /// </summary>
    private Transform ropeOrigin = null;

    /// <summary>
    /// The vertical speed of the player when they let go of the rope.
    /// </summary>
    public float letGoSpeed = 20;

    void Start () {
        pawn = GetComponent<PawnAABB>();
        DeriveJumpValues();
        _rigidbody = GetComponent<Rigidbody2D>();
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
    void DeriveJumpValues()
    {
        gravity = (jumpHeight * 2) / (jumpTime * jumpTime);
        jumpVelocity = gravity * jumpTime;
    }
	/// <summary>
    /// This method is called each frame. 
    /// </summary>
	void Update ()
    {
        if (_rigidbody.bodyType == RigidbodyType2D.Kinematic)
        {
            HandleInput();
            DoCollisions();
        }
        if (_rigidbody.bodyType == RigidbodyType2D.Dynamic)
        {
            InputForSwing();
        }
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
        isGrounded = results.hitBottom || results.onSlope;
        transform.position += results.distance;
    }
    /// <summary>
    /// This method uses input to manipulate this object's physics.
    /// </summary>
    private void HandleInput()
    {
        GravityAndJumping();

        float axisH = Input.GetAxisRaw("Horizontal");

        if (axisH == 0)
        {
            DecelerateX(walkAcceleration);
        }
        else
        {
            AccelerateX(axisH * walkAcceleration);
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
    /// This is the input for swingning on a rope, bitch!
    /// </summary>
    private void InputForSwing()
    {
        float axisH = Input.GetAxisRaw("Horizontal");
        Vector2 dir = transform.position - ropeOrigin.position;
        dir = new Vector2(-dir.y, dir.x);
        dir.Normalize();
        _rigidbody.AddForce(dir * axisH * swingStrength);
        if (Input.GetButtonDown("Jump"))
        {
            RopeSpawn rope = ropeOrigin.gameObject.GetComponent<RopeSpawn>();
            rope.UnlinkRope();
            ropeOrigin = null;
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            velocity = _rigidbody.velocity;
            _rigidbody.velocity = Vector2.zero;
            velocity.y += letGoSpeed;
        }
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
    /// <param name="amount">The scalar value of horizontal acceleration. This should be a positive number.</param>
    private void AccelerateX(float amount)
    {
        velocity.x += amount * Time.deltaTime;
    }

    /// <summary>
    /// This sets an origin for swinging on a rope.
    /// </summary>
    /// <param name="origin"></param>
    public void SetOriginForRope(Transform origin)
    {
        ropeOrigin = origin;
    }
}

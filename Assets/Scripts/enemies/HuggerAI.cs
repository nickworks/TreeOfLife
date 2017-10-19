using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PawnAABB))]
public class HuggerAI : MonoBehaviour
{

    /// <summary>
    /// The acceleration to use for gravity. This will be calculated from the jumpTime and jumpHeight fields.
    /// </summary>
    private float gravity = .5f;

    private Vector2 velocity = Vector2.zero;
    private PawnAABB pawn;
    private bool isGrounded = false;
    public bool isBurrowed = false;
    public bool isBurrowing = false;
    private bool isLeaping = false;
    private float timeUnderground = 0;
    private Vector2 preBurrowPoint;
    private const float SURFACE_AT = 2;
    private Vector2 jumpVelocity = new Vector2(5, 10);
    private const float BURROW_SPEED = 2.0f;
    private const float MAX_DISTANCE = 3.0f;
    private const float BURROW_DEPTH = 1;
    private int playerLockDirection = 0;

    // Use this for initialization
    void Start ()
    {
        pawn = GetComponent<PawnAABB>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        GravityAndStuff();

        if (isBurrowing)
            BurrowHandler();
        else if (isLeaping)
            HandleLeaping();
        else
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

        if (isGrounded && Vector2.Distance(PlayerController.main.transform.position, transform.position) < MAX_DISTANCE)
        {
            isBurrowing = true;
            preBurrowPoint = transform.position;
        }

        transform.position += results.distance;
    }

    void GravityAndStuff()
    {
        if (!isGrounded && !isBurrowing && !isLeaping)
            velocity.y -= gravity;
    }

    void BurrowHandler()
    {
        if (isBurrowed)
        {
            velocity = Vector2.right * BURROW_SPEED * PlayerDirection();
            isGrounded = false;

            if (timeUnderground < SURFACE_AT)
                timeUnderground += Time.deltaTime;
            else
            {
                isBurrowing = false;
                isBurrowed = false;
                isLeaping = true;
                velocity = jumpVelocity;
                playerLockDirection = PlayerDirection();
            }
        }
        else
        {
            if (transform.position.y > preBurrowPoint.y - BURROW_DEPTH)
                velocity.y = -1;
            else
                isBurrowed = true;
        }

        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    void HandleLeaping()
    {        
        velocity.x = Mathf.Abs(velocity.x) * playerLockDirection;
        velocity.y -= gravity;

        transform.position += (Vector3)velocity * Time.deltaTime;

        if (velocity.y <= 0)
            isLeaping = false;
    }

    int PlayerDirection()
    {
        return PlayerController.main.transform.position.x > transform.position.x ? 1 : -1;
    }
}

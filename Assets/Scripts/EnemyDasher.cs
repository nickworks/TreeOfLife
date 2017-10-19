using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This component turns a GameObject into a and enemy that can be Thrown.
/// </summary>
[RequireComponent(typeof(PawnAABB))]
public class EnemyDasher : MonoBehaviour
{
    /// <summary>
    /// bool
    /// </summary>
    public bool inRange = false;
    /// <summary>
    /// vector3
    /// </summary>
    public Vector3 targetPos;
    /// <summary>
    /// float
    /// </summary>
    public float enemeyImpulse;
    /// <summary>
    /// vector3
    /// </summary>
    private Vector3 velocity = new Vector3();
    /// <summary>
    /// pawnAABB
    /// </summary>
    private PawnAABB pawn;
    /// <summary>
    /// float
    /// </summary>
    private float gravity = 10;
    /// <summary>
    /// bool
    /// </summary>
    static public bool isGrounded = false;
    /// <summary>
    /// bool
    /// </summary>
    public bool canDash = true;
    /// <summary>
    /// bool
    /// </summary>
    public bool isHit = true;
    /// <summary>
    /// bool
    /// </summary>
    private bool moveRight = true;
    /// <summary>
    /// float
    /// </summary>
    private float moveCount;
    /// <summary>
    /// float
    /// </summary>
    public float moveCountReset = 10;
    /// <summary>
    /// float
    /// </summary>
    public float speed = 10;


    /// <summary>
    /// method that only runs once and is used to initiliaze things
    /// </summary>
    void Start()
    {
        pawn = GetComponent<PawnAABB>();
        moveCount = moveCountReset;
       
    }

    /// <summary>
    /// this method runs every frame and is used to call other methodes and check to see if trhe player is in range of the object
    /// </summary>
    void Update()
    {
        Move();
        DecelerateX(20);
        DoCollisions();       
        if (inRange)
        {                 
            Dash();
        }
        float gravityScale = 2;
        velocity.y -= gravity * Time.deltaTime * gravityScale;
    }

    /// <summary>
    /// this method is used to "Throw" the enemy  
    /// </summary>
    private void Dash()
    {
        if (isHit)
        {
            if (canDash)
            {
                if (Input.GetButtonDown("Fire2"))
                {
                    velocity = DirectionalDash.rot * enemeyImpulse;
                    canDash = false;

                }
            }
        }
    }
    /// <summary>
    /// This method is used to move the object back and forth untill dead
    /// </summary>
    private void Move()
    {
        if (!isHit)
        {
            if (moveRight)
            {
                velocity.x += speed * Time.deltaTime;
                moveCount -= Time.deltaTime;
                if (moveCount <= 0)
                {
                    moveRight = false;
                    moveCount = moveCountReset;

                }

            } else {
                velocity.x += -speed * Time.deltaTime;
                moveCount -= Time.deltaTime;
                if (moveCount <= 0)
                {
                    moveRight = true;
                    moveCount = moveCountReset;

                }
            }
        }
    }
    /// <summary>
    /// This method accelerates the horizontal speed of the object.
    /// </summary>
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
    private void AccelerateX(float amount)
    {
        velocity.x += amount * Time.deltaTime;
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
}

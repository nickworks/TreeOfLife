using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PawnAABB))]
public class EnemyDasher : MonoBehaviour
{
    public bool inRange = false;
    public Vector3 targetPos;
   
   // Vector3 currentPos;
    public float enemeyImpulse;
    private Vector3 velocity = new Vector3();
    private PawnAABB pawn;
    private float gravity = 10;
     private bool isGrounded = false;
    public bool canDash = true;
    public bool isHit = false;
    private bool moveRight = true;
   private float moveCount;
    public float moveCountReset = 10;
    public float speed = 10;



    void Start()
    {
        pawn = GetComponent<PawnAABB>();
        moveCount = moveCountReset;
       
    }

   
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
    private void DoCollisions()
    {
        PawnAABB.CollisionResults results = pawn.Move(velocity * Time.deltaTime);
        if (results.hitTop || results.hitBottom) velocity.y = 0;
        if (results.hitLeft || results.hitRight) velocity.x = 0;
        isGrounded = results.hitBottom || results.ascendSlope;
        transform.position += results.distance;
    }
}

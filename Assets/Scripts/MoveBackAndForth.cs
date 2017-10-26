using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackAndForth : MonoBehaviour {
    /// <summary>
    /// pawnAABB used for collition detection
    /// </summary>
    private PawnAABB pawn;
    /// <summary>
    /// bool used to see if the opbject is grounded
    /// </summary>
    private bool isGrounded = false;
    /// <summary>
    /// vector3 used to store and change the objects veloscity 
    /// </summary>
    private Vector3 velocity = new Vector3();
    /// <summary>
    /// bool used to tell if the object has been hit
    /// </summary>
  private bool isHit = true;
    /// <summary>
    /// boolused to determin what direction to move the object ehn t is alive
    /// </summary>
    private bool moveRight = true;
    /// <summary>
    /// float used to determin the number of seconds untill the directioin is changed
    /// </summary>
    private float moveCount;
    /// <summary>
    /// float used to determin the number of seconds untill the directioin is changed
    /// </summary>
    public float moveCountReset = 10;
    /// <summary>
    /// float used to determin the speed of the object
    /// </summary>
    public float speed = 10;
    // Use this for initialization
    void Start () {
        pawn = GetComponent<PawnAABB>();
        moveCount = moveCountReset;
    }
	
	// Update is called once per frame
	void Update () {
        DoCollisions();
        Move();
        DecelerateX(20);
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

            }
            else
            {
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
    private void DoCollisions()
    {
        PawnAABB.CollisionResults results = pawn.Move(velocity * Time.deltaTime);
        if (results.hitTop || results.hitBottom) velocity.y = 0;
        if (results.hitLeft || results.hitRight) velocity.x = 0;
        isGrounded = results.hitBottom || results.ascendSlope;
        transform.position += results.distance;
    }
}

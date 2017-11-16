using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This component turns a GameObject into a and enemy that can be Thrown.
/// </summary>
[RequireComponent(typeof(PawnAABB3D))]
public class EnemyDasher : MonoBehaviour
{
    /// <summary>
    /// bool used to tell if the target is in range 
    /// </summary>
    public bool inRange = false;
    /// <summary>	/// 
    /// vector3 used to store the targets position 
    /// </summary>
    public Vector3 targetPos;
    /// <summary>
    /// float used to store the throw power 
	/// </summary>
    public float enemeyImpulse;
    /// <summary>
    /// vector3 used to store and change the objects velocity 
    /// </summary>
    private Vector3 velocity = new Vector3();
    /// <summary>
    /// pawnAABB used for collision detection
    /// </summary>
    private PawnAABB3D pawn;
    /// <summary>
    /// float used to give gravity to the object
    /// </summary>
    private float gravity = 10;
    /// <summary>
    /// bool used to see if the opbject is grounded
    /// </summary>
    private bool isGrounded = false;
    /// <summary>
    /// bool used to tell whether or not the object can be thrown
    /// </summary>
    public bool canBeThrown = true;
    /// <summary>
    /// bool used to tell if the object has been hit
    /// </summary>
    public bool isHit = true;

    /// <summary>
    /// method that only runs once and is used to initiliaze things
    /// </summary>
    void Start()
    {
        pawn = GetComponent<PawnAABB3D>();       
    }
	/// <summary>	
    /// this method runs every frame and is used to call other methods and check to see if the player is in range of the object
    /// </summary>
    void Update()
    {
        DoCollisions();
        DecelerateX(20);
        if (inRange)
        {                 
            Throw();
        }
        float gravityScale = 2;
        if (velocity.y == 0) {
            canBeThrown = true;

        }
        velocity.y -= gravity * Time.deltaTime * gravityScale;
    }
    /// <summary>
    /// this method is used to "Throw" the enemy  
    /// </summary>
    private void Throw()
    {
        if (isHit)
        {
            if (canBeThrown)
            {
                if (Input.GetButtonDown("Fire2"))
                {
                    //velocity = DirectionalDash.rot * enemeyImpulse;
                    print("ahhhhhh");
                    
                    canBeThrown = false;
                }
            }
        }
    }
    /// <summary>
    /// This method Decelerates the horizontal speed of the object.
    /// </summary>
    private void DecelerateX(float amount)
    {
        // slow down the object
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
        PawnAABB3D.CollisionResults results = pawn.Move(velocity * Time.deltaTime);
        if (results.hitTop || results.hitBottom) velocity.y = 0;
        if (results.hitLeft || results.hitRight) velocity.x = 0;
        isGrounded = results.hitBottom || results.ascendSlope;
        transform.position += results.distanceLocal;
    }
}

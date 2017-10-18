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
    public bool isBurrowing = false;
    private bool isLeaping = false;
    private float timeUnderground = 0;
    private const float SURFACE_AT = 2;
    private const Vector2 JUMP_VELOCITY = new Vector2(20, 40);

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
        transform.position += results.distance;
    }

    void GravityAndStuff()
    {
        if (!isGrounded && !isBurrowing)
            velocity.y -= gravity;
    }

    void BurrowHandler()
    {
        pawn.Move(velocity * Time.deltaTime);
        isGrounded = false;

        if (timeUnderground < SURFACE_AT)
            timeUnderground += timeUnderground.deltaTime;
        else
        {
            isBurrowing = false;
            isLeaping = true;
        }
    }

    void HandleLeaping()
    {
        pawn.Move(velocity * Time.deltaTime);
        velocity.y = JUMP_SPEED;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PawnAABB3D))]
public class HuggerAI : MonoBehaviour
{

    /// <summary>
    /// The acceleration to use for gravity. This will be calculated from the jumpTime and jumpHeight fields.
    /// </summary>
    private float gravity = .5f;

    /// <summary>
    /// The velocity of the enemy.
    /// </summary>
    private Vector2 velocity = Vector2.zero;
    /// <summary>
    /// The reference to the pawnAABB component.
    /// </summary>
    private PawnAABB3D pawn;
    /// <summary>
    /// Whether the enemy is grounded.
    /// </summary>
    private bool isGrounded = false;
    /// <summary>
    /// Whether the enemy is fully underground.
    /// </summary>
    public bool isBurrowed = false;
    /// <summary>
    /// Whether the enemy is burrowing or is burrowed.
    /// </summary>
    public bool isBurrowing = false;
    /// <summary>
    /// Whether the enemy is leaping upward. Turns to false when falling.
    /// </summary>
    private bool isLeaping = false;
    /// <summary>
    /// Currently unused variable for how long the enemy has been underground.
    /// </summary>
    private float timeUnderground = 0;
    /// <summary>
    /// The point that the enemy starts before burrowing.
    /// </summary>
    private Vector3 preBurrowPoint;
    /// <summary>
    /// Unused constant for when the enemy should surface and leap when burrowed.
    /// </summary>
    private const float SURFACE_AT = 2;
    /// <summary>
    /// Temporary velocity to jump at the player with.
    /// </summary>
    private Vector2 jumpVelocity = new Vector2(5, 10);
    /// <summary>
    /// The speed to burrow through the ground.
    /// </summary>
    private const float BURROW_SPEED = 2.0f;
    /// <summary>
    /// The maximum distance from the player to trigger activation.
    /// </summary>
    private const float MAX_DISTANCE = 5.0f;
    /// <summary>
    /// How far to burrow into the ground.
    /// </summary>
    private const float BURROW_DEPTH = 1;
    /// <summary>
    /// The remembered x direction that the player was.
    /// </summary>
    private int playerLockDirection = 0;

    private bool isHugging = false;

    private AlignWithPath aligner;

    // Use this for initialization
    void Start ()
    {
        pawn = GetComponent<PawnAABB3D>();
        aligner = GetComponent<AlignWithPath>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        GravityAndStuff();

        if (isHugging)
        {

        }
        else if (isBurrowing)
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
        PawnAABB3D.CollisionResults results = pawn.Move(velocity * Time.deltaTime);
        if (results.hitTop || results.hitBottom) velocity.y = 0;
        if (results.hitLeft || results.hitRight) velocity.x = 0;
        isGrounded = results.hitBottom || results.ascendSlope;

        float distanceFromPlayer = Vector3.Distance(Player.PlayerController.main.transform.position, transform.position);

        if (isGrounded)
        {
            if (distanceFromPlayer < MAX_DISTANCE)
            {
                isBurrowing = true;
                preBurrowPoint = transform.position;
            }
        }
        else if (distanceFromPlayer < 1)
            SetHugging();

        if (!isHugging)
            transform.position += results.distanceLocal;
    }

    /// <summary>
    /// Handles gravity.
    /// </summary>
    void GravityAndStuff()
    {
        if (!isGrounded && !isBurrowing && !isLeaping)
            velocity.y -= gravity;
    }

    /// <summary>
    /// Sets the enemy up so that it hugs the player.
    /// </summary>
    void SetHugging()
    {
        isHugging = true;
        isLeaping = false;
        transform.SetParent(Player.PlayerController.main.transform);
    }

    /// <summary>
    /// Handsles bburrowing into the ground.
    /// </summary>
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

    /// <summary>
    /// Handles leaping into the air at the player.
    /// </summary>
    void HandleLeaping()
    {        
        velocity.x = Mathf.Abs(velocity.x) * playerLockDirection;
        velocity.y -= gravity;

        transform.position += (Vector3)velocity * Time.deltaTime;

        float distanceFromPlayer = Vector3.Distance(Player.PlayerController.main.transform.position, transform.position);
        if (distanceFromPlayer < 1)
            SetHugging();

        if (velocity.y <= 0)
            isLeaping = false;
    }

    /// <summary>
    /// Finds the direction the player is at.
    /// </summary>
    /// <returns></returns>
    int PlayerDirection()
    {
        AlignWithPath playerAligner= Player.PlayerController.main.gameObject.GetComponent<AlignWithPath>();

        if (playerAligner.currentNode == aligner.currentNode)
        {
            float vx = aligner.pathPercent();
            return playerAligner.pathPercent() > vx ? 1 : playerAligner.pathPercent() < vx ? -1 : 0;
        }
        else if (playerAligner.currentNode == aligner.currentNode.left)
        {
            Debug.Log("WHYYYYYYY?");
            return -1;
        }
        else if (playerAligner.currentNode == aligner.currentNode.right)
        {
            return 1;
        }
        return 0;
    }
}

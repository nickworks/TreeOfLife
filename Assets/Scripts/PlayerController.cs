using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component turns a GameObject into a controllable avatar.
/// </summary>
[RequireComponent(typeof(PawnAABB))]
public class PlayerController : MonoBehaviour {
    ///
    ///A variable to control the players climb speed
    ///
    public float climbSpeed = 10.5f;

    /// <summary>
    /// A transform to hold the spawnpoints transform
    /// </summary>
    public Transform spawnTransform;
    /// <summary>
    /// A variable used to control when the spawnpoint moves
    /// </summary>
    public bool moveSpawn;
    /// <summary>
    /// A variable used to hold the amount of Vertical propulsion a player recieves from being shot from a slingshot web
    /// </summary>
    public float verticalPropulsion;
    /// <summary>
    /// A variable used to hold the amount of Horizontal propulsion a player recieves from being shot from a slingshot web
    /// </summary>
    public float horizontalPropulsion;
    /// <summary>
    /// A speed to hold the degree by which the propulsion is increased
    /// </summary>
    public float propulsionSpeed;
    /// <summary>
    /// A variable to hold which way the player should be propelled
    /// </summary>
    public int propulsionDirection;

    /// <summary>
    /// A boolean to hold whether or not the character is being propelled through the air
    /// </summary>
    public bool flying;

    ///
    ///A variable used to give the player jump velocity in the web
    ///
    public float webJumpVelocity = 40;

    ///
    /// A Boolean to dictate when a player is in a web or not
    /// 
    public bool inWeb;
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
    void DeriveJumpValues()
    {
        gravity = (jumpHeight * 2) / (jumpTime * jumpTime);
        jumpVelocity = gravity * jumpTime;
    }
    /// <summary>
    /// This method is called each frame. 
    /// </summary>
    void Update()
    {
       // print(spawnTransform);
       // print(propulsionDirection);
        //If not in web
        if (!inWeb && !flying)
        {
            HandleInput();
            DoCollisions();
        }//End of If not in web, If we are in the web movement is handeled by climbing
        else if (inWeb && !flying)
        {
            //Calls climb
            climb();
            DoCollisions();
        } else if (flying)
        {
            //Sling shot the player if they are flying through the air
            SlingShot();
            DoCollisions();//Do collisions to move the player
            
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
        isGrounded = results.hitBottom || results.ascendSlope;
        transform.position += results.distance;
    }
    /// <summary>
    /// This method uses input to manipulate this object's physics.
    /// </summary>
    private void HandleInput()
    {
        GravityAndJumping();
        Spawning();
        float axisH = Input.GetAxisRaw("Horizontal");
        float axisV = Input.GetAxisRaw("Vertical");
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

    private void Spawning()
    {
        if (Input.GetButtonDown("Respawn"))
        {
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

    /// <summary>
    /// A OnTrigger stay event to handle when the player is inside a trigger
    /// </summary>
    /// <param name="collision"> This is a collision variable</param>
    private void OnTriggerStay2D(Collider2D collision)
    {

        //If the player is inside of the sticky web
        if(collision.gameObject.tag == "StickyWeb")
        {
           // print("COLLIDING");
            //Debug.Log("COLLIDING");
            //Set web to true
            flying = false;
            inWeb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "StickyWeb")
        {
            
            inWeb = false;
        }
        else if (collision.gameObject.tag == "Spawn")
        {
            print("OUTOFSPAWN");
            moveSpawn = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If the player collides with the slingshotweb set  flying to true
        if (collision.gameObject.tag == "SlingShotWeb")
        {
            flying = true;
            
        } else if (collision.gameObject.tag == "Ground")//If the player collides with the ground flying is false
        {
            flying = false;
        }else if(collision.gameObject.tag == "Stopper")//If the player collides with the stopper flying is false
        {
            flying = false;
            
        }else if(collision.gameObject.tag == "Spawn")
        {
            print("HERE IN SPAWN");
            moveSpawn = true;
        }
    }

    /// <summary>
    /// A private method to hold the logic for being slingshot
    /// </summary>
    private void SlingShot()
    {
        //Depending on the propulsion direction it changes the way the player is propelled
        if (propulsionDirection == 1)
        {
            velocity.y += verticalPropulsion * Time.deltaTime;
            velocity.x += horizontalPropulsion * Time.deltaTime;
        } else if(propulsionDirection == 2)
        {
            velocity.y += verticalPropulsion * Time.deltaTime;
            velocity.x -= horizontalPropulsion * Time.deltaTime;
        }
        else if(propulsionDirection == 3)
        {
            //If propulsionDirection is 3 we want the player to be shot straight up
            velocity.y += (verticalPropulsion + 20) * Time.deltaTime;
            
        }

          
    }

    //This is a method to handle climbing in the web
    public void climb()
    {
        //This is a method call to handle jumping in the web
        //JumpingInWeb();
        //Gets the players raw movement axis
        float axisH = Input.GetAxisRaw("Horizontal");
        float axisV = Input.GetAxisRaw("Vertical");
        //sets the velocity x and y to climbspeed times the movement axis
        velocity.x = climbSpeed * axisH * Time.deltaTime;
        velocity.y = climbSpeed * axisV * Time.deltaTime;
        
    }

    #region OUtdatedCode
    /// <summary>
    /// Allowed the player to jump in the web to speed up testing
    /// </summary>
    //private void JumpingInWeb()
    // {



    //if (Input.GetButtonDown("Jump"))
    // {

    //  velocity.y = webJumpVelocity;
    //  isJumping = true;
    //
    // }


    // transform.position += velocity * Time.deltaTime;
    // gravity
    //velocity.y -= gravity * Time.deltaTime * gravityScale;
    // }
#endregion

}
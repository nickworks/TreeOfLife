using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyWebController : MonoBehaviour {
    /// <summary>
    /// A reference to the player controller object
    /// </summary>
    PlayerController pc;

    /// <summary>
    /// A private boolean to dictate whether or not the player has grabbed the web
    /// </summary>
    private bool playerGrabbed;

    /// <summary>
    /// A public float used to control the players climb speed. This is added to the player controllers Velocity.y, and Velocity.X
    /// </summary>
    public float climbSpeed;
    /// <summary>
    /// A public float used to control the players jump velocity. Added to the Player Controllers Velocity.y
    /// </summary>
    public float jumpVelocity;
    /// <summary>
    /// A public float used to slow the player down;
    /// </summary>
    public float decelerationAmount;

    // Use this for initialization
    void Start () {
        /// <summary>
        /// Gets the player controller object
        /// </summary>
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }
	
	// Update is called once per frame
	void Update () {
        /// <summary>
        /// If the player has grabbed the web
        /// </summary>
        if (playerGrabbed)
        {
            //Handle the climbing movement in the web
            HandleClimbing();
        }
        
	}

    /// <summary>
    /// A private method to handle Climbing movement
    /// </summary>
    private void HandleClimbing()
    {
        //A call to the handle jump method
        HandleJump();
        //A float that recieves the Horizontal axis Raw
        float axisH = Input.GetAxisRaw("Horizontal");
        //A float that recieves the vertical axis raw
        float axisV = Input.GetAxisRaw("Vertical");
        //The players velocity x is equal to the horizontal axis multiplied by the climbspeed multiplied by deltatime
        pc.velocity.x += axisH * climbSpeed *Time.deltaTime;
        //the player velocity y is equal to the vertical axis multiplied by the climbspeed multiplied by deltatime
        pc.velocity.y += axisV * climbSpeed * Time.deltaTime;
        //Decelerates climbing
        ClimbingDecelerate();
    }//The 



    /// <summary>
    /// A private method to handle climbing deceleration
    /// </summary>
    private void ClimbingDecelerate()
    {
        //The players velocity x is multiplied by a deceleration amount
        pc.velocity.x *= decelerationAmount;
        //the players velocity y is multiplied by a deceleration amount
        pc.velocity.y *= decelerationAmount;
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            playerGrabbed = false;
            pc.DeriveJumpValues();
            pc.velocity.y += jumpVelocity * Time.deltaTime;
        }

    }

    /// <summary>
    /// A private method to handle collision enter events
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /// <summary>
        /// A switch statement to handle the many collision events
        /// </summary>
        switch (collision.gameObject.tag)
        {   //The player case
            case "Player":
               
                if (pc.grabbing)
                {
                    //Changes the playerGrabbed variable to true to represent player grabbing the web
                    playerGrabbed = true;
                    //Sets the players gravity to 0
                    pc.gravity = 0;
                    //Sets the players velocity to 0
                    pc.velocity.y = 0;


                    
                }
               
                
                break;//End of player case
        }//End of switch statement

    }//End of OnTriggerEnter2D method for Sticky Webs



    /// <summary>
    /// A private method to handle collision stay events
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {

        /// <summary>
        /// A switch statement to handle the many collision events
        /// </summary>
        switch (collision.gameObject.tag)
        {   //The player case
            case "Player":
                //If the player is pressing the grab button
                if (!playerGrabbed && pc.grabbing)
                {
                    //Changes the playerGrabbed variable to true to represent player grabbing the web
                    playerGrabbed = true;
                    //Sets the players gravity to 0
                    pc.gravity = 0;
                    //Sets the players velocity to 0
                    pc.velocity.y = 0;

                    
                }


                break;//End of player case
        }//End of switch statement

    }//End of OnTriggerStay2D Method for sticky Webs


    /// <summary>
    /// A private method to handle collision stay events
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        /// <summary>
        /// A switch statement to handle the many collision events
        /// </summary>
        switch (collision.gameObject.tag)
        {   //The player case
            case "Player":
                    //Calls the player controllers DeriveJumpValues method so we get reguler gravity
                    pc.DeriveJumpValues();
                    //Sets it so the player is no longer grabbing the web
                    playerGrabbed = false;
                


                break;//End of player case
        }
    }//End of OnTriggerExit2D method for sticky webs


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a public class to handle the logic for the webs the player can grab and stick too
/// </summary>
public class StickyWebController : MonoBehaviour {
    /// <summary>
    /// A reference to the player controller object
    /// </summary>
    Player.PlayerController pc;

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

    /// <summary>
    /// A private float used so the player has a delay before they can grab back onto webs
    /// </summary>
    private float grabTimer;

    // Use this for initialization
    void Start ()
    {
        /// <summary>
        /// Gets the player controller object
        /// </summary>
      // pc = GameObject.Find("Player").GetComponent<Player.PlayerController>();
    }
	/*
	// Update is called once per frame
	void Update ()
    {
        //If the grabTimer is greater than 0
       // if(grabTimer > 0)
       // {
            //Subtract the grab timer by deltatime
        //    grabTimer -= Time.deltaTime;
       // }
        /// <summary>
        /// If the player has grabbed the web
        /// </summary>
       // if (playerGrabbed)
      //  {
            //Handle the climbing movement in the web
       //     HandleClimbing();
            
       // }
        
	}

    /// <summary>
    /// A private method to handle Climbing movement
    /// </summary>
    private void HandleClimbing()
    {
        //A call to the handle jump method
       // HandleJump();
        //A float that recieves the Horizontal axis Raw
       // float axisH = Input.GetAxisRaw("Horizontal");
        //A float that recieves the vertical axis raw
       // float axisV = Input.GetAxisRaw("Vertical");
        //The players velocity x is equal to the horizontal axis multiplied by the climbspeed multiplied by deltatime
       // pc.velocity.x += axisH * climbSpeed *Time.deltaTime;
        //the player velocity y is equal to the vertical axis multiplied by the climbspeed multiplied by deltatime
       // pc.velocity.y += axisV * climbSpeed * Time.deltaTime;
        //Decelerates climbing
       // ClimbingDecelerate();

        //This is code to get the player to drop from the web
       // if(Input.GetButton("Drop") && Input.GetButton("Action"))
        //{
            //Calls the player controllers DeriveJumpValues method so we get reguler gravity
           // pc.DeriveJumpValues();
            //Sets it so the player is no longer grabbing the web
          //  playerGrabbed = false;

        //}//End of code to get the player to drop from the web
    }//The 



    /// <summary>
    /// A private method to handle climbing deceleration
    /// </summary>
    private void ClimbingDecelerate()
    {
        //The players velocity x is multiplied by a deceleration amount
       // pc.velocity.x *= decelerationAmount;
        //the players velocity y is multiplied by a deceleration amount
      //  pc.velocity.y *= decelerationAmount;
    }
    /// <summary>
    /// This handles the players jumping while they are in the web
    /// </summary>
    private void HandleJump()
    {
        // If input is equal to jump
       // if (Input.GetButtonDown("Jump"))
       // {
            //We set the playerGrabbed variable to false to represent the player letting go of the web
            //playerGrabbed = false;
            //We derive the players jump values to get gravity back
            //pc.DeriveJumpValues();
            //We adjust the players y velocity by the jump velocity variable multiplied by delta time
           // pc.velocity.y += jumpVelocity * Time.deltaTime;
       // }

    }

    /// <summary>
    /// A private method to handle collision enter events
    /// </summary>
    /// <param name="collision"></param>
   // private void OnTriggerEnter2D(Collider2D collision)
    //{
        /// <summary>
        /// A switch statement to handle the many collision events
        /// </summary>
        //switch (collision.gameObject.tag)
        //{   //The player case
          //  case "Player":
               
                //If the player is grabbing and grab timer is less than or equal to 0
              //  if (pc.grabbing && grabTimer <= 0)
               // {
                    //Set the grabTimer
                   // grabTimer = .75f;
                   // //Changes the playerGrabbed variable to true to represent player grabbing the web
                  //  playerGrabbed = true;
                    //Sets the players gravity to 0
                  //  pc.gravity = 0;
                    //Sets the players velocity to 0
                  //  pc.velocity.y = 0;                   
              //  }
               
                
             //   break;//End of player case
       // }//End of switch statement

    }//End of OnTriggerEnter2D method for Sticky Webs


    /*
    /// <summary>
    /// A private method to handle collision stay events
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            
        }
        /// <summary>
        /// A switch statement to handle the many collision events
        /// </summary>
        //switch (collision.gameObject.tag)
       // {   //The player case
           // case "Player":
             //   //If the player is pressing the grab button and grab timer is less than or equal to 0
               // if (!playerGrabbed && pc.grabbing && grabTimer <= 0)
                //{
                    //Set the grabTimer delay
                  //  grabTimer = .75f;
                    //Changes the playerGrabbed variable to true to represent player grabbing the web
                   // playerGrabbed = true;
                    //Sets the players gravity to 0
                  //  pc.gravity = 0;
                  ///  //Sets the players velocity to 0
                   // pc.velocity.y = 0;

                    
               // }


             //   break;//End of player case
        //}//End of switch statement

    }//End of OnTriggerStay2D Method for sticky Webs


   /* /// <summary>
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
    */

}

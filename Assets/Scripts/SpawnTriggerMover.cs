using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A public class for the SpawnTriggerMover which moves the volume used to set the next checkpoint
/// </summary>
public class SpawnTriggerMover : MonoBehaviour {


    /// <summary>
    /// A public int used to control where the spawn volume goes next
    /// </summary>
    public int spawnState = 0;
    /// <summary>
    /// A public boolean used to tell us when to move the spawn
    /// </summary>
    public bool moveSpawn;
    /// <summary>
    /// A vector 3 to hold the new spawn position
    /// </summary>
    public Vector3 newPosition = new Vector3();


    /// <summary>
    /// A private method to handle trigger collision events
    /// </summary>
    /// <param name="collision"> A reference to the collider interacting with this object</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /// <summary>
        /// A switch statement to handle the many collision events
        /// </summary>
        switch (collision.gameObject.tag)
        {   //The player case
            case "Player":
                //Lets us know to move the spawn
                moveSpawn = true;

                //Increases the spawn state variable
                spawnState++;

                
              break;//End of player case
        }//End of switch statement

    }//End of OnTriggerEnter2D method for Sticky Webs

    /// <summary>
    /// A private method to handle when an object exits collision volumes
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
                //sets the boolean back to false
                moveSpawn = false;
                //Adjusts the new spawn position based on the spawn state
                switch (spawnState)
                {
                    case 1:
                        //Adjusts newPosition Vector
                        newPosition.y = 16;
                        //Sets transform to newPosition Vector
                        transform.position = newPosition;
                        break;
                    case 2:
                        //Adjusts newPosition Vector
                        newPosition.y = 8.15f;
                        newPosition.x = -34.75f;
                        //Sets transform to newPosition Vector
                        transform.position = newPosition;
                        break;
                    case 3:
                        //Adjusts newPosition Vector
                        newPosition.x = -64;
                        //Sets transform to newPosition Vector
                        transform.position = newPosition;
                        break;
                   
                }

                //Sets the transform position equal to the new position
                transform.position = newPosition;


                break;//End of player case
        }//End of switch statement
    }
}

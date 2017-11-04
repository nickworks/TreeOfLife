using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Falling Platform is a class that creates a falling platform on any object this attached to.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class FallingPlatform : MonoBehaviour
{

    /// <summary>
    /// Stores a Reference
    /// </summary>
    private Rigidbody rb3d;

    /// <summary>
    /// Boolean - IsFalling tracks to see if the object is falling 
    /// </summary>
    private bool isFalling = false;

    /// <summary>
    /// downSpeed is the way that speed is calculated for downfalling objects
    /// </summary>
    private float downSpeed = 0;

    /// <summary>
    /// Vector3 variable stores original position of object.
    /// </summary>
    private Vector3 OriginalPosition;

    /// <summary>
    ///  Gives Original Position a local position
    /// </summary>
    void Start()
    {
        OriginalPosition = transform.localPosition;
    }
    // Use this for initialization//FIXME - what is this <
    /// <summary>
    /// Checks to see if the player is tagged. If it is a player the object is set to falling
    /// if not it isfalling is false!
    /// </summary>
    /// <param name="collider"></param>
    public void OnTriggerEnter(Collider collider)
    {
        /// Tag of Player
        if (collider.CompareTag("Player"))
        {
            print("Hello");

            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
    }
    /// <summary>
    /// Updates and checks to see if the object is falling is checked for true and the transform position is decreased by downspeed by += time.deltatime / 100;
    /// TO DO : Add destroy object
    /// </summary>
    void Update()
    {
        if (isFalling)
        {
            print("fuck");
            downSpeed += Time.deltaTime / 100;
            transform.position = new Vector3(transform.position.x, transform.position.y - downSpeed, transform.position.z);

        }

        // Once the object hits a Y position it stops and is no longer interactable 
        if (transform.localPosition.y < -20)
        {
            downSpeed += Time.deltaTime / 15;
            //Transform Local Position is reset but is increased before hitting the final area.
            if (transform.localPosition.y < -40)
            {
                isFalling = false;
                transform.position = OriginalPosition;
                downSpeed = 0;
            }

        }

    }

}

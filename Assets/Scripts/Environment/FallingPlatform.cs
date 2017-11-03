using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Falling Platform is a class that creates a falling platform on any object this attached to.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class FallingPlatform : MonoBehaviour {
    
    /// <summary>
    /// gets a reference to the rigid body
    /// </summary>
    private Rigidbody2D rb2d;
    /// <summary>
    /// isfalling the main interaction item here
    /// </summary>
    public bool isFalling = false;
    /// <summary>
    /// downSpeed is the way that speed is calculated for downfalling objects
    /// </summary>
    public float downSpeed = 0;

    // Use this for initialization
    /// <summary>
    /// Tag of Player
    /// Checks to see if the player is tagged. If it is a player the object is set to falling
    /// if not it isfalling is false!
    /// </summary>
    /// <param name="collider"></param>
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {

            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
    }
    /// <summary>
    /// Update
    /// Updates and checks to see if the object is falling is checked for true and the transform position is decreased by downspeed by += time.deltatime / 100;
    /// Once the object hits a Y position it stops and is no longer interactable
    /// TO DO : Add destroy object
    /// </summary>
    void Update()
    {
        if(isFalling)
        {
            downSpeed += Time.deltaTime / 100;
            transform.position = new Vector3(transform.position.x, transform.position.y - downSpeed, transform.position.z);

        }
        if (transform.localPosition.y < -40)
        {
            isFalling = false;
            Destroy(this);
            
        }
        
    }
  
}

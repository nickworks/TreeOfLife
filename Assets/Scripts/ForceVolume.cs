using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A trigger script for collision volumes that applies forces to objects that enter the volume.  Requires some kind of 2D collider!
/// </summary>
public class ForceVolume : MonoBehaviour {

    /// <summary>
    /// How strong the force is in this particular volume 
    /// </summary>
    [Tooltip("The strength of the force in this volume")]
    public float forceMult = 2f;
    /// <summary>
    /// What direction the force of this volume goes.  Should be normalized.
    /// </summary>
    [Tooltip("The direction of force in this volume. Will be normalized.")]
    public Vector3 forceVector = Vector3.up;
    /// <summary>
    /// Should this volume overwrite the other object's gravity, or simply add to it's velocity?
    /// </summary>
    [Tooltip("Should the force be treated as temporary gravity, or simple velocity?")]
    public bool OverwritesGravity = false;
    /// <summary>
    /// Controls whether this volume will apply force to pawns or not.
    /// </summary>
    [Tooltip("Is the volume currently applying it's forces?")]
    public bool turnedOn = true;
    /// <summary>
    /// Controls whether the volume toggles between active and inactive on a timer
    /// </summary>
    [Tooltip("Should this volume toggle on/off on a timer?")]
    public bool isTimed = false;
    /// <summary>
    /// The number (in seconds) between deactivating and reactivating (the inactive time)
    /// </summary>
    [Tooltip("How long (in seconds) the volume is turned ON during a timer cycle.")]
    public float offTime = 1f;
    /// <summary>
    /// The number (in seconds) between activating and deactivating (the active time)
    /// </summary>
    [Tooltip("How long (in seconds) the volume is turned ON during a timer cycle.")]
    public float activeTime = 1f;
    /// <summary>
    /// The timer controling toggling (used if isTimed = true)
    /// </summary>
    private float timer = 0f;

    /// <summary>
    /// Every tick, if the "is timed" option is checked, the game cycles the timer and adjusts the volume accordingly
    /// </summary>
    private void Update() {
        if( isTimed ) {

            timer -= Time.deltaTime;

            if(timer < 0f ) {
                turnedOn = !turnedOn;//toggle the active state
                timer = turnedOn ? activeTime : offTime;//apply the time to the timer

                GetComponent<MeshRenderer>().enabled = turnedOn;//toggle visibility of the volume
            }
        }
    }

    /// <summary>
    /// What to do when an object enters this trigger volume.  Overwrites gravity if that option is checked.
    /// </summary>
    /// <param name="collision">The object that collided with this volume.</param>
    private void OnTriggerEnter2D( Collider2D collision )
    {
        if( OverwritesGravity )
        {
            //only apply this to the player pawns
            if( collision.tag == "Player" )
            {
                collision.GetComponent<Player.PlayerController>().SetGravity(forceVector, forceMult);
            }
        }        
    }
    /// <summary>
    /// What to do when an object exits this trigger volume.  Resets gravity forces that may have been changed to defaults.
    /// </summary>
    /// <param name="collision">The object exiting the volume</param>
    private void OnTriggerExit2D( Collider2D collision )
    {
        if( OverwritesGravity )
        {
            if( collision.tag == "Player" )
            {
                collision.GetComponent<Player.PlayerController>().SetGravity();
            }
        }
    }
    /// <summary>
    /// What to do every tick an object is in this volume.  Applies forces if overwrites gravity is false.
    /// </summary>
    /// <param name="collision">The object that collided with this volume.</param>
    private void OnTriggerStay2D( Collider2D collision )
    {
        if( !OverwritesGravity )
        {
            if( collision.tag == "Player" )
            {
                collision.GetComponent<Player.PlayerController>().ApplyForce(forceMult, forceVector);
            }
        }
    }

}

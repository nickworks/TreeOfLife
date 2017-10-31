using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

/// <summary>
/// This class controls a proximity trigger volume for enemies. This component MUST be on a gameobject that is a
/// direct child of a gameobject with a sub-class component of BehaviourNPC.
/// </summary>
public class TriggerNPC : MonoBehaviour {

    /// <summary>
    /// Method that detects when collision happens with this trigger
    /// </summary>
    /// <param name="collision">The 2D colliding object</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            //print("Chase!");
            GetComponentInParent<BehaviorNPC>().FindsPlayer(collision.GetComponent<PlayerController>());
        }
    }
    /// <summary>
    /// Method constantly updates while a collision is happening
    /// </summary>
    /// <param name="collision">The 2D colliding object</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GetComponentInParent<BehaviorNPC>().PlayerNearby(collision.GetComponent<PlayerController>());
        }
    }
    /// <summary>
    /// Method that detects when collision is no longer happening
    /// </summary>
    /// <param name="collision">The 2D colliding object</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            GetComponentInParent<BehaviorNPC>().LosesPlayer();
        }
        
    }
}

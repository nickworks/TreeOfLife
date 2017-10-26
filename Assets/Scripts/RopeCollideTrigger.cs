using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class is put onto ropes or other interactable objects for the player to climb on
/// </summary>
public class RopeCollideTrigger : MonoBehaviour {
    /// <summary>
    /// holds a reference to the player object
    /// </summary>
    private Player.PlayerController pawn;
    /// <summary>
    /// When the player is colliding with a rope, checks inputs for climb hotkey
    /// </summary>
    /// <param name="collider"></param>
	void OnTriggerStay2D (Collider2D collider) {
        if (collider.tag == "Player" && !pawn.isClimbing)
        {
            print("it's colliding");
            if (Input.GetButtonDown("Climb"))
            {
                pawn.climbBuffer = .2f;
                pawn.isClimbing = true;
            }
        }
	}
    /// <summary>
    /// If the player is climbing a rope and falls off of it, this will make the player stop climbing.
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerExit2D (Collider2D collider)
    {
        if (collider.tag == "Player" && pawn.isClimbing)
        {
            pawn.isClimbing = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

/// <summary>
/// Every NPC that will use a TriggerNPC will need to be derived from this class.
/// </summary>
public abstract class BehaviorNPC : MonoBehaviour {

    /// <summary>
    /// This method is called by a TriggerNPC when the player enters a trigger volume on that object.
    /// </summary>
    /// <param name="player">The player</param>
    public abstract void FindsPlayer(PlayerController player);
    /// <summary>
    /// This method is called by a TriggerNPC when the player remains in a trigger volume on that object.
    /// </summary>
    /// <param name="player">The player</param>
    public abstract void PlayerNearby(PlayerController player);
    /// <summary>
    /// This method is called by a TriggerNPC when the player leaves a trigger volume on that object.
    /// </summary>
    /// <param name="player">The player</param>
    public abstract void LosesPlayer();
}

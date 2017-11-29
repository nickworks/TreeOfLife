using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the logic for a basic kill volume in the environment. If the player collides with the volume
/// the player will "die" and be respawned at the last checkpoint, if any.
/// </summary>
public class KillVolume : MonoBehaviour
{

    /// <summary>
    /// Reference to the SpawnPoint object in the scene
    /// </summary>
    GameObject spawnRef;

    /// <summary>
    /// Used to set the reference
    /// </summary>
    private void Start()
    {
        spawnRef = GameObject.Find("SpawnPoint");
    }

    /// <summary>
    /// Checks if the player has collided and if they do, "kill" them and respawn them
    /// </summary>
    /// <param name="other">Collided with object</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.SetActive(false);
            other.GetComponent<Player.PlayerController>().velocity = Vector3.zero;
            other.transform.localPosition = spawnRef.transform.localPosition;
            other.GetComponent<AlignWithPath>().currentNode = spawnRef.GetComponent<SpawnLocation>().spawnNode;
            other.gameObject.SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Checkpoint class is used to set checkpoint markers for the player to respwan at if they are to die
/// It sets all the relavent information to the SpwanLocation and SpawnPoint gameobject 
/// </summary>
public class Checkpoint : MonoBehaviour {

    /// <summary>
    /// This holds a reference to the SpwanPoint Gameobject in the scene
    /// </summary>
    public GameObject spawnPoint;

    // Use this for initialization
    void Start () {
        spawnPoint = GameObject.Find("SpawnPoint");
	}
	
    /// <summary>
    /// Check if the player has collided with this checkpoint and set the SpwanPoint location to it
    /// </summary>
    /// <param name="other">Object that collides with this one (in this case the player object)</param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            spawnPoint.GetComponent<SpawnLocation>().spawnNode = other.GetComponent<AlignWithPath>().currentNode;
            spawnPoint.transform.localPosition = transform.localPosition;

        }
    }

}

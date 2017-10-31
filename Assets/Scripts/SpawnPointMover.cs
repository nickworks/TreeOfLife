using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A public class to hold the logic for Moving the players spawn point itself
/// </summary>
public class SpawnPointMover : MonoBehaviour {

    /// <summary>
    /// A variable to hold a rigidbody reference
    /// </summary>
    Rigidbody2D rb;
  
    /// <summary>
    /// A variable to hold the spawnTrigger Mover reference
    /// </summary>
    SpawnTriggerMover stm;

	// Use this for initialization
	void Start ()
    {
        //Getting rigid body reference
        rb = GetComponent<Rigidbody2D>();
       
        //Getting the Spawn Trigger Mover regerence
        stm = GameObject.Find("SpawnTrigger").GetComponent<SpawnTriggerMover>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //If the move variable in the SpawnTriggerMover script is set to true then we need to move the spawn
        if (stm.moveSpawn) MoveSpawn();
	}

    private void MoveSpawn()
    {
        //We set the rigidbody transform equal to the STM transform
        rb.transform.position = stm.transform.position;
    }
}

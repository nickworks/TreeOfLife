using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointMover : MonoBehaviour {

    Rigidbody2D rb;
    PlayerController pc;
    
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (pc.moveSpawn) MoveSpawn();
	}

    private void MoveSpawn()
    {
        rb.transform.position = pc.transform.position;
    }
}

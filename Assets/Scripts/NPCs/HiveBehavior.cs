using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveBehavior : MonoBehaviour {

    /// <summary>
    /// A reference to the FlyBehavior script that we will use to instantiate the Flies
    /// </summary>
    public FlyBehavior fly;

    /// <summary>
    /// A list of bug game objects
    /// </summary>
    List<FlyBehavior> flies = new List<FlyBehavior>();

    /// <summary>
    /// A reference to the players transform
    /// </summary>
    public Transform playerTransform;

    /// <summary>
    /// A reference to a secondary target for the bugs to follow
    /// </summary>

    // Use this for initialization
    void Start () {
        //TODO: Randomize fly positions
        //TODO: Make it so flies are able to follow a leader
        
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
		while(flies.Count < 5)
        {
            FlyBehavior newFly = Instantiate(fly, transform.position, transform.rotation);
            newFly.target = playerTransform;
            flies.Add(newFly);
        }
        
	}
	
	// Update is called once per frame
	void Update () {
        flies[1].isStopped = true;
    }
}

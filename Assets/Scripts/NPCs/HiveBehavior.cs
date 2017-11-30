<<<<<<< HEAD
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveBehavior : MonoBehaviour {

    /// <summary>
    /// A vector used to randomize the bugs starting position
    /// </summary>
	Vector3 randomize = new Vector3();

    /// <summary>
    /// The number of flies to be spawned
    /// </summary>
    public int spawnNumber;

    /// <summary>
    /// The state of the hive
    /// </summary>
    public int state;

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
    public Transform secondaryTarget;


    // Use this for initialization
    void Start ()
    {       
       playerTransform = GameObject.Find("Player").GetComponent<Transform>();		     
	}
	
	// Update is called once per frame
	void Update () {
        
        while (flies.Count < spawnNumber)
        {
            randomize.x = Random.Range(1, 5);
            randomize.y = Random.Range(1, 5);
            randomize.z = Random.Range(1, 5);
            FlyBehavior newFly = Instantiate(fly, transform.position + randomize, transform.rotation);
            if(state == 1)
            {
                newFly.target = playerTransform;
                secondaryTarget = newFly.GetComponent<Transform>();
                state++;
            }else if(state == 2)
            {
                newFly.target = secondaryTarget;
            }
            flies.Add(newFly);
        }
    }
}
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveBehavior : MonoBehaviour {

    /// <summary>
    /// A vector used to randomize the bugs starting position
    /// </summary>
	Vector3 randomize = new Vector3();

    /// <summary>
    /// The number of flies to be spawned
    /// </summary>
    public int spawnNumber;

    /// <summary>
    /// The state of the hive
    /// </summary>
    public int state;

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
    public Transform secondaryTarget;


    // Use this for initialization
    void Start ()
    {       
       playerTransform = GameObject.Find("Player").GetComponent<Transform>();		     
	}
	
	// Update is called once per frame
	void Update () {
        
        //TODO: Comment
        while (flies.Count < spawnNumber)
        {
            randomize.x = Random.Range(1, 5);
            randomize.y = Random.Range(1, 5);
            randomize.z = Random.Range(1, 5);
            FlyBehavior newFly = Instantiate(fly, transform.position + randomize, transform.rotation);
            if(state == 1)
            {
                newFly.target = playerTransform;
                secondaryTarget = newFly.GetComponent<Transform>();
                state++;
            }else if(state == 2)
            {
                newFly.target = secondaryTarget;
            }
            flies.Add(newFly);
        }
    }
}
>>>>>>> master

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A public class for the Hive Controller Object
/// </summary>
public class HiveController : MonoBehaviour {

    /// <summary>
    /// A list of bug game objects
    /// </summary>
    List<GameObject> bugs = new List<GameObject>();
    /// <summary>
    /// A game object to hold the bug prefabs
    /// </summary>
    public GameObject prefabBugs;
    /// <summary>
    /// A variable to hold refereces to the fly controller script
    /// </summary>
    FlyController fc;

	// Use this for initialization
	void Start ()
    {
        //Getting the reference to the flycontroller script on the leaderbug game object
        fc = GameObject.Find("LeaderBug").GetComponent<FlyController>();
	}
	  
    /// <summary>
    /// A private void to handle Trigger collision events
    /// </summary>
    /// <param name="collision"> A variable to handle collisions</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /// <summary>
        /// A switch statement to handle the many collision events
        /// </summary>
        switch (collision.gameObject.tag)
        {   //The leaderbug case case
            case "LeaderBug":
                //Sets the alertHive variable on the leaderbug object to false
                fc.alerrtHive = false;
                //Sets the rage variable on the leaderbug object to true
                fc.rage = true;
                //Sets the aggroRange to 15
                fc.aggroRange = 15;
                //While bugs.Count is less than five
                while (bugs.Count < 5)
                {
                    //Instantiate a new bug
                    GameObject newBug = Instantiate(prefabBugs, transform.position, Quaternion.identity);
                    //Add the newBug to the bugs list
                    bugs.Add(newBug);
                }


                break;//End of player case
        }//End of switch statement
    }

}

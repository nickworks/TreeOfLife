using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A public class for the Hive Controller Object
/// </summary>
public class HiveController : MonoBehaviour {

	/// <summary>
	/// A string to set the leaderbugs name;
	/// </summary>
	public string LeaderBugName;

    /// <summary>
    /// A vector used to randomize the bugs starting position
    /// </summary>
	Vector3 randomize = new Vector3();

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
    

	/// <summary>
	/// An integer to hold the number of bugs the hive spawns
	/// </summary>
	public int bugLimit;

	/// <summary>
	/// A variable to hold the behavior of the hive
	/// </summary>
	public int state;

	/// <summary>
	/// Start the bugs spawning
	/// </summary>
	public bool action;

    /// <summary>
    /// A variable used to control if the bugs can spawn
    /// </summary>
    public bool canSpawn;

	// Use this for initialization
	void Start ()
    {
        //Getting the reference to the flycontroller script on the leaderbug game object
		fc = GameObject.Find(LeaderBugName).GetComponent<FlyController>();
        //Actions is false by default
        action = false;
        //canSpawn is set to true by default
        canSpawn = true;

		//randomize.x = Random.Range (1, 4);
	}

    //This is updated everyframe
	void Update()
	{
        //If state is one and action is true
		if (state == 1 && action == true)
		{
            //While the bugs lists count is less than the bugs limit
			while(bugs.Count < bugLimit)
			{
                //Randomize the bugs position
                randomize.x = Random.Range(1, 5);
                randomize.y = Random.Range(1, 5);
                //Instantiate bugs and add them to the bugs list
                GameObject newBug = Instantiate (prefabBugs, transform.position + randomize, Quaternion.identity);
                bugs.Add(newBug);
			}	

		}

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
              
               if(state == 2)//Else if state is true
                {

                    //While bugs.Count is less than five
                    while (bugs.Count < bugLimit)
                    {
                        //Randomize a position
                        randomize.x = Random.Range(1, 5);
                        randomize.y = Random.Range(1, 5);
                        //Instantiate a new bug
                        GameObject newBug = Instantiate(prefabBugs, transform.position + randomize, Quaternion.identity);
                        //Add the newBug to the bugs list
                        bugs.Add(newBug);

                    }
                }          
                break;//End of player case
            case "Player":
                //If the hives state is one
                if(state == 1)
                {
                    //Action is set to true
                    action = true;
                }
                break;
        }//End of switch statement
    }

}

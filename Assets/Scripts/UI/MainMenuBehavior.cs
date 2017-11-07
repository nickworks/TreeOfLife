using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    /// <summary>
    /// Starts the Game
    /// </summary>
   public void GameStart()
    {
        SceneManager.LoadScene("main");
    }

    /// <summary>
    /// Exits the game
    /// </summary>
    public void GameExit()
    {
        print("Greetings this is being implemented");
    //    SceneManager.
    }
}

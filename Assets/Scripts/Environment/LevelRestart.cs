using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Level Restart is a Modular Tool that allows developers to restart the level or as a final game ending.
/// 
/// </summary>
public class LevelRestart : MonoBehaviour {

    public string Level;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            SceneManager.LoadScene(Level);

        }

    }
}

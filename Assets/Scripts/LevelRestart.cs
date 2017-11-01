using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Level Restart is a Modular Tool that allows developers to restart the level or as a final game ending.
/// This is a modular tool
/// Use : Please use the string tag to customize what level you wish to restart
/// requires the trigger box ticked in order to work
/// Along with a rigidbody2d/boxcollider
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class LevelRestart : MonoBehaviour {
    /// <summary>
    /// Level is the string attached to change the behavior of what scene to load up
    /// </summary>
    public string Level;

    /// <summary>
    /// OnTriggerEnter2d 
    /// Checks to see if the collider is a player.
    /// If it is true the scene manager loads a scene
    /// This is meant for debugging or loading a new level
    /// </summary>
    /// <param name="collider"></param>
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            SceneManager.LoadScene(Level);

        }

    }
}

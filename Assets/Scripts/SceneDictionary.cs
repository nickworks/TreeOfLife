using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is intended to be used for loading different levels. Making variables here will make things easy to update in the future
/// especially if levels are called in other areas of code. They just need to be update here and they are good to go.
/// This class should be added to a persistant object so it can be used in all scenes.
/// </summary>
public class SceneDictionary : MonoBehaviour {

    /// <summary>
    /// Add levels that will be used as static public strings here. Don't forget the levels need to be added to the building settings for the methods to work
    /// </summary>
    static public List<string> scenes = new List<string> { "3dtrack" };

    /// <summary>
    /// This will hold a reference to the current scene for whatever it may be used for, starting the level over, ect.
    /// </summary>
    static public string currentScene = "";

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// This method is used for loading new scenes and setting the current scene to the new scene
    /// </summary>
    /// <param name="scene"></param>
    public void LoadScene(string scene)
    {
        currentScene = scene;
        SceneManager.LoadScene(scene);
    }
    /// <summary>
    /// This method is used for loading new scenes and setting the current scene to the new scene
    /// </summary>
    /// <param name="sceneNumber">This is the index of the scene in the list scenes to be loaded</param>
    public void LoadScene(int sceneNumber)
    {
        string newScene = scenes[sceneNumber];
        if(newScene != null)
        {
            currentScene = newScene;
            SceneManager.LoadScene(newScene);
        }
        
    }

    /// <summary>
    /// Method that can be used to reload the current scene 
    /// </summary>
    public void RestartLevel()
    {
        if(currentScene != null)
            LoadScene(currentScene);
    }
}

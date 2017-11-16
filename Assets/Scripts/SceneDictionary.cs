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
    /// Add levels that will be used as static public strings here. Don't forget they need to be added to the building settings for the methods to work
    /// </summary>
    static public string mainHub = "";
    static public string testingGrounds = "3dtrack";

    /// <summary>
    /// This will hold a reference to the current scene for whatever it may be used for, starting the level over, ect.
    /// </summary>
    static public string currentScene = "";

    /// <summary>
    /// This method is used for loading new scenes and setting the current scene to the new scene
    /// </summary>
    /// <param name="scene"></param>
    void LoadScene(string scene)
    {
        currentScene = scene;
        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// These are the methods that will be used to load different scenes, especailly for use in editor with buttons
    /// Just add a new method for loading a new scene as needed
    /// </summary>
    public void LoadMainHub()
    {
        LoadScene(mainHub);
    }

    public void LoadTestingGrounds()
    {
        LoadScene(testingGrounds);
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

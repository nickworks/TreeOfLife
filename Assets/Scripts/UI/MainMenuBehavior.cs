using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBehavior : MonoBehaviour {

    /// <summary>
    /// Starts the Game
    /// </summary>
   public void GameStart()
    {
        SceneManager.LoadScene("LevelSelection");
    }

   
    /// <summary>
    /// Exits the game
    /// </summary>
    public void GameExit()
    {
        //if unity_Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
               print("Greetings this is being implemented");
    //    SceneManager.
    }

    public void SaveVolumeSetting(float value)
    {
        PlayerPrefs.SetFloat("volume", value);
        PlayerPrefs.Save();

        //check API for unity
    }
}

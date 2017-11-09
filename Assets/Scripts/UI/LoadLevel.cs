using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour {

    /// <summary>
    /// Scene01 is a string that is how we connect.
    /// </summary>
    public string Scene01;



    /// <summary>
    /// GameLevelLoad loads a scene 
    /// </summary>
    public void GameLevelLoad()
    {
        SceneManager.LoadScene(Scene01);
    }


}

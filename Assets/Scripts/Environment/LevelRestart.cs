using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(BoxCollider))]
///Level Restart
///
public class LevelRestart : MonoBehaviour
{
    /// <summary>
    /// Public String of level, where user types in level. //will be changed
    /// </summary>
    public string Level;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// On Collision with Player new scene is loaded
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            SceneManager.LoadScene(Level);

        }

    }
}

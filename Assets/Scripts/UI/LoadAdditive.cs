using UnityEngine;
using System.Collections;

public class LoadAdditive : MonoBehaviour
{
    /// <summary>
    /// loads a level or adds them to a level counter
    /// </summary>
    /// <param name="level"></param>
    public void LoadAddOnClick(int level)
    {
        Application.LoadLevelAdditive(level);
    }
}
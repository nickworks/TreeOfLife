using UnityEngine;
using System.Collections;

public class LoadAdditive : MonoBehaviour
{

    public void LoadAddOnClick(int level)
    {
        Application.LoadLevelAdditive(level);
    }
}
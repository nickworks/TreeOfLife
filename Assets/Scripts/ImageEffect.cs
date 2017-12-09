using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEffect : MonoBehaviour {

    public static bool active = false;
    public Material mat;

    void OnRenderImage(RenderTexture src, RenderTexture des)
    {
        if (active)
        {
            Graphics.Blit(src, des, mat);
        }
        else
        {
            Graphics.Blit(src, des);
        }
    }
    /*private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            active = !active;
        }
    }*/
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is made to render a post processing effect on the screen for "Spirit Vision'
/// This script goes on the main camera and requires a material with a post process shader
/// </summary>
public class ImageEffect : MonoBehaviour {

    /// <summary>
    /// Should "Spirit Vision" or the screen effect be active?
    /// </summary>
    public static bool active = false;
    /// <summary>
    /// Material with a post processing shader on it
    /// </summary>
    public Material mat;

    /// <summary>
    /// This method allows a post process shader to be applied to the camera and render it out on screen
    /// </summary>
    /// <param name="src">The source image</param>
    /// <param name="des">The destination image</param>
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
}

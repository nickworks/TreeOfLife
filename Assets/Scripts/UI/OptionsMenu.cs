using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

    /// <summary>
    /// Audiomixer accesses master volume
    /// </summary>
    public AudioMixer audioMixer;


    /// <summary>
    /// Accesses dropdown resolutions
    /// </summary>
    public Dropdown resolutionDropdown;

    /// <summary>
    /// Array of Resolutions
    /// </summary>
    Resolution[] resolutions;

    /// <summary>
    /// does not work 
    /// </summary>
    ///  public float GammaCorrection;

    /// <summary>
    /// does not work 
    /// </summary>
    public Rect SliderLocation;


    /// <summary>
    /// This checks to see what the resolutions are and automatically retrieves a list of resolutions
    /// This is for the dropdown menu for the resolutions
    /// </summary>
    private void Start()
    {
     //?   RenderSettings.ambientLight = new Color(GammaCorrection, GammaCorrection, GammaCorrection, 1.0f);
        DropdownMenuResolutions();
    }
    /// <summary>
    /// This is the resolutions tab for the game.
    /// This automatically updates the resolution tabs for use in the options menu
    /// </summary>
    private void DropdownMenuResolutions()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    /// <summary>
    /// Sets the Volume on a slider
    /// </summary>
    /// <param name="volume"></param>
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    /// <summary>
    /// Controls Brightness // still being written
    /// </summary>
    /// <param name="volume"></param>
    public void Brightness(float volume)
    {
     //   GammaCorrection = GUI.HorizontalSlider(SliderLocation, GammaCorrection, 0, 1.0f);
    }

    /// <summary>
    /// Controls contrast // still be written
    /// </summary>
    /// <param name="volume"></param>
    public void Contrast(float volume)
    {

    }

    /// <summary>
    /// Set the Quality of the game
    /// </summary>
    /// <param name="qualityIndex"></param>
    public void SetQuality (int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    /// <summary>
    /// Set the game to fullscreen
    /// </summary>
    /// <param name="isFullScreen"></param>
    public void setFullscreen (bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    /// <summary>
    /// Sets Resolution of computer 
    /// </summary>
    /// <param name="resolutionIndex"></param>
    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}

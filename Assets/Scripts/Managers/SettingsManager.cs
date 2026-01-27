using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public SettingsManager Instance;
    public AudioMixer audioMixer; // replace with your custom audio stuff later, idk if its in the proj yet
    private float masterVolumePercent;
    private float musicVolumePercent;
    private float sfxVolumePercent;
    private float minVolume = -80f;
    private float maxVolume = 20f;
    private List<float> zoomLevels = new List<float> (1, 2, 3); // can be removed if you want it to be a slider
    private List<float> gameSpeeds = new List<float> (0.5f, 0.75f, 1f, 1.25f, 1.5f);

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }


    // Audio functions
    public void SetMasterVolume(float percent)
    {
        if (percent >= 0 && percent <= 1)
        {
            masterVolumePercent = percent;
            audioMixer.SetFloat("volume", Mathf.Lerp(minVolume, maxVolume, masterVolumePercent));
            Debug.Log($"Master volume set to {masterVolumePercent}");
        }
    }

    // these are just examples, fill in later when i know how the audio middleware works
    public void SetMusicVolume(float percent)
    {
        if (percent >= 0 && percent <= 1)
        {
            // fill in as needed 
        }
    }
    public void SetSFXVolume(float percent)
    {
        if (percent >= 0 && percent <= 1)
        {
            // fill in as needed 
        }
    }

    // Screen settings
    public void SetFullScreen(bool newFullscreen)
    {
        Screen.fullScreen = newFullscreen;
    }
    public void SetResolution(int width, int height) // thought these values could be passed through the UI in a dropdown or something
    {
        Screen.SetResolution(width, height, Screen.fullScreen);
    }

    public void SetZoomLevel (int index)
    {
        if (index < zoomLevels.Count);
            // CameraManager.Instance.SetZoom(zoomLevels[index]);
    }
    public void SetGameSpeed (int index)
    {
        if (index < gameSpeeds.Count());
            // GameManager.Instance.SetGameSpeed(gameSpeeds[index]);
    }

    // i dont really think we need texture settings (resolution and AA) since its all pixel art

    // Accessibility
    public void SetTextSize (float scale) // same as above with dropdown
    {
        // icl this seems like a huge pain, might not do
    }
    public void SetColourBlindMode (int index)
    {
        // I think this can just be handled through applying a shader, i remember seeing a video about them before
    }

}

using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    public AudioMixer audioMixer; // replace with your custom audio stuff later, idk if its in the proj yet
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float uiVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float gameplayVolume = 1f;
    public float gameSpeed = 1f;
    private float minGameSpeed = 0.5f;
    private float maxGameSpeed = 2f;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }


    // Audio functions
    // *** REWRITE LATER TO SUPPORT THE FMOD MIGRATION ***
    // public void SetMasterVolume(float newVolume)
    // {
    //     masterVolume = newVolume;
    //     AudioManagerFMOD.Instance.masterVolume = masterVolume;
    // }
    // public void SetUIVolume(float newVolume)
    // {
    //     uiVolume = newVolume;
    //     AudioManagerFMOD.Instance.uiVolume = uiVolume;
    // }
    // public void SetMusicVolume(float newVolume)
    // {
    //     musicVolume = newVolume;
    //     AudioManagerFMOD.Instance.musicVolume = musicVolume;
    // }
    // public void SetGameplayVolume(float newVolume)
    // {
    //     gameplayVolume = newVolume;
    //     AudioManagerFMOD.Instance.gameplayVolume = gameplayVolume;
    // }

    // Screen settings
    public void SetFullScreen(bool newFullscreen)
    {
        Screen.fullScreen = newFullscreen;
    }
    public void SetResolution(int width, int height) // thought these values could be passed through the UI in a dropdown or something
    {
        Screen.SetResolution(width, height, Screen.fullScreen);
    }

    public void SetZoomLevel (float zoomPercent)
    {
        CameraManager.Instance.UpdateCameraZoom(zoomPercent);
    }
    public void SetGameSpeed (float speedPercent)
    {
        gameSpeed = Mathf.Lerp(minGameSpeed, maxGameSpeed, speedPercent);
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

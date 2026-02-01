using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class SettingsMenu : UIMenuBase
{
    private Slider masterVolumeSlider;
    private Slider musicVolumeSlider;
    private Slider fxVolumeSlider;
    private Slider gameplayVolumeSlider;
    private Slider zoomLevelSlider;
    private Slider gameSpeedSlider;
    private Button backButton;

    protected override void OnCreate()
    {
        // Set up references to the UI elements
        masterVolumeSlider = Root.Q<Slider>("MasterVolumeSlider");
        musicVolumeSlider = Root.Q<Slider>("MusicVolumeSlider");
        fxVolumeSlider = Root.Q<Slider>("FXVolumeSlider");
        gameplayVolumeSlider = Root.Q<Slider>("GameplayVolumeSlider");
        zoomLevelSlider = Root.Q<Slider>("ZoomLevelSlider");
        gameSpeedSlider = Root.Q<Slider>("GameSpeedSlider");
        backButton = Root.Q<Button>("BackButton");

        // Button event handlers
        masterVolumeSlider.RegisterValueChangedCallback(v => { SettingsManager.Instance.SetMasterVolume(v.newValue); });
        musicVolumeSlider.RegisterValueChangedCallback(v => { SettingsManager.Instance.SetMusicVolume(v.newValue); });
        fxVolumeSlider.RegisterValueChangedCallback(v => { SettingsManager.Instance.SetUIVolume(v.newValue); });
        gameplayVolumeSlider.RegisterValueChangedCallback(v => { SettingsManager.Instance.SetGameplayVolume(v.newValue); });
        zoomLevelSlider.RegisterValueChangedCallback(v => { SettingsManager.Instance.SetZoomLevel(v.newValue); });
        gameSpeedSlider.RegisterValueChangedCallback(v => { SettingsManager.Instance.SetGameSpeed(v.newValue); });

    }

}

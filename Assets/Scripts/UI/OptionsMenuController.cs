using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptionsMenuController : MonoBehaviour
{
    private int _originalResolution;
    private int _originalQuality;
    private int _originalScreenMode;
    private int _chosenResolution;
    private int _chosenQuality;
    private int _chosenScreenMode;
    private bool _isVerticalSyncActive;

    private Resolution[] _resolutions;
    [SerializeField] private TMP_Dropdown _screenModeDropdown;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMP_Dropdown _qualityDropdown;
    [SerializeField] private GameObject _verticalSyncCheckMark;

    void Start()
    {
        _chosenResolution = -1;
        _chosenQuality = -1;
        _chosenScreenMode = -1;
        _isVerticalSyncActive = false;

        _resolutionDropdown.ClearOptions();
        List<string> options = new();
        _resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + "x" + _resolutions[i].height + " " + _resolutions[i].refreshRateRatio + "Hz";
            options.Add(option);
            if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        _screenModeDropdown.RefreshShownValue();
        _resolutionDropdown.AddOptions(options);
        _resolutionDropdown.value = currentResolutionIndex;
        _resolutionDropdown.RefreshShownValue();
        _qualityDropdown.value = QualitySettings.GetQualityLevel();
        _qualityDropdown.RefreshShownValue();

        LoadSettings();

        Application.targetFrameRate = 120;
    }

    public void ChooseScreenMode(int screenModeIndex)
    {
        _chosenScreenMode = screenModeIndex;
        _screenModeDropdown.value = screenModeIndex;
        _screenModeDropdown.RefreshShownValue();
    }

    public void SetChosenScreenMode()
    {
        if (_chosenScreenMode == -1)
            return;
        
        FullScreenMode newScreenMode = _chosenScreenMode switch
        {
            0 => FullScreenMode.Windowed,
            1 => FullScreenMode.FullScreenWindow,
            2 => FullScreenMode.ExclusiveFullScreen,
            _ => FullScreenMode.Windowed
        };
        Resolution resolution = Screen.currentResolution;
        Screen.SetResolution(resolution.width, resolution.height, newScreenMode);
        
        Debug.Log("ScreenMode.fullScreenMode has been set to: " + Screen.fullScreenMode + " (" + _chosenScreenMode + ")");
        Debug.Log("ScreenMode.fullScreen has been set to: " + Screen.fullScreen + " (" + _chosenScreenMode + ")");
        _originalScreenMode = _chosenScreenMode;
        _screenModeDropdown.RefreshShownValue();
    }

    public void ChooseResolution(int resolutionIndex)
    {
        _chosenResolution = resolutionIndex;
        _resolutionDropdown.value = resolutionIndex;
        _resolutionDropdown.RefreshShownValue();
    }

    public void SetChosenResolution()
    {
        if (_chosenResolution == -1)
            return;

        _originalResolution = _chosenResolution;
        Resolution resolution = _resolutions[_chosenResolution];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        _resolutionDropdown.RefreshShownValue();
    }

    public void ChooseQuality(int qualityIndex)
    {
        _chosenQuality = qualityIndex;
        _qualityDropdown.value = qualityIndex;
        _qualityDropdown.RefreshShownValue();
    }

    public void SetChosenQuality()
    {
        if (_chosenQuality == -1)
            return;

        _originalQuality = _chosenQuality;
        QualitySettings.SetQualityLevel(_chosenQuality);
        _qualityDropdown.RefreshShownValue();
    }

    public void ToggleVerticalSync(bool active)
    {
        if (active)
        {
            _verticalSyncCheckMark.SetActive(true);
            _isVerticalSyncActive = true;
        }
        else
        {
            _verticalSyncCheckMark.SetActive(false);
            _isVerticalSyncActive = false;
        }
    }

    public void ToggleVerticalSyncDependingOnCheckMark()
    {
        if (_verticalSyncCheckMark.activeSelf)
        {
            ToggleVerticalSync(false);
        }
        else
        {
            ToggleVerticalSync(true);
        }
    }

    public void SetVerticalSync()
    {
        if (_isVerticalSyncActive)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }

    public void ChooseOriginalSettings()
    {
        ChooseScreenMode(_originalScreenMode);
        ChooseResolution(_originalResolution);
        ChooseQuality(_originalQuality);
    }

    public void ApplySettings()
    {
        SetChosenScreenMode();
        SetChosenResolution();
        SetChosenQuality();
        SetVerticalSync();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("QualitySettingPreference", _qualityDropdown.value);
        PlayerPrefs.SetInt("ScreenModePreference", (int)Screen.fullScreenMode);
        PlayerPrefs.SetInt("ResolutionPreference", _resolutionDropdown.value);
        PlayerPrefs.SetInt("VerticalSyncPreference", _isVerticalSyncActive ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("QualitySettingPreference"))
        {
            int qualityIndex = PlayerPrefs.GetInt("QualitySettingPreference");
            ChooseQuality(qualityIndex);
        }
        else
        {
            int qualityIndex = 3;
            _originalQuality = qualityIndex;
            ChooseQuality(qualityIndex);
        }

        if (PlayerPrefs.HasKey("ScreenModePreference"))
        {
            int screenModeIndex = PlayerPrefs.GetInt("ScreenModePreference");
            switch (screenModeIndex)
            {
                case 0:
                    ChooseScreenMode(2);
                    break;
                case 1:
                    ChooseScreenMode(1);
                    break;
                case 3:
                    ChooseScreenMode(0);
                    break;
                default:
                    ChooseScreenMode(0);
                    break;
            }
            _originalScreenMode = screenModeIndex;
        }
        else
        {
            int screenModeIndex = 0;
            _originalScreenMode = screenModeIndex;
            ChooseScreenMode(screenModeIndex);
        }

        if (PlayerPrefs.HasKey("ResolutionPreference"))
        {
            int resolutionIndex = PlayerPrefs.GetInt("ResolutionPreference");
            ChooseResolution(resolutionIndex);
            _originalResolution = resolutionIndex;
        }
        else
        {
            int resolutionIndex = _resolutions.Length - 1;
            _originalResolution = resolutionIndex;
            ChooseResolution(resolutionIndex);
        }

        if (PlayerPrefs.HasKey("VerticalSyncPreference"))
        {
            int verticalSyncIndex = PlayerPrefs.GetInt("VerticalSyncPreference");
            if (verticalSyncIndex == 1)
            {
                ToggleVerticalSync(true);
            }
            else
            {
                ToggleVerticalSync(false);
            }
        }

        ApplySettings();
    }

    public void ShowSettings()
    {
        ChooseOriginalSettings();
        gameObject.SetActive(true);
    }

    public void HideSettings()
    {
        gameObject.SetActive(false);
    }
}

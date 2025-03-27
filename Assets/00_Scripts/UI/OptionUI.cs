using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private ArrowOptionUI languageOptionUI;
    [SerializeField] private ArrowOptionUI resolutionOptionUI;
    [SerializeField] private ArrowOptionUI isWindowOptionUI;
    [SerializeField] private ArrowOptionUI frameOptionUI;
    [SerializeField] private ArrowOptionUI vSyncOptionUI;
    [SerializeField] private ArrowOptionUI backgroundVolumeUI;
    [SerializeField] private ArrowOptionUI sfxVolumeUI;

    [SerializeField] private Button cancelButton;

    private void Awake()
    {
        cancelButton.onClick.AddListener(HideOptionUI);
        InitArrowOptions();
    }

    private void OnEnable()
    {
        languageOptionUI.SetIndex(GameSettingManager.instance.gameSettingData.langaugeIndex);
        resolutionOptionUI.SetIndex(GameSettingManager.instance.gameSettingData.resolutionIndex);
        isWindowOptionUI.SetIndex(GameSettingManager.instance.gameSettingData.isWindowIndex);
        frameOptionUI.SetIndex(GameSettingManager.instance.gameSettingData.frameIndex);
        vSyncOptionUI.SetIndex(GameSettingManager.instance.gameSettingData.vSyncIndex);
        backgroundVolumeUI.SetIndex(GameSettingManager.instance.gameSettingData.backgroundSoundVolume);
        sfxVolumeUI.SetIndex(GameSettingManager.instance.gameSettingData.sfxSoundVolume);
    }

    private void InitArrowOptions()
    {
        List<string> options = new List<string>();

        for (int i = 0; i < GameSettingDefinition.languageList.Count; i++)
        {
            options.Add(GameSettingDefinition.languageList[i]);
        }

        languageOptionUI.SetOptions(options);
        languageOptionUI.onValueChanged += ChangeLanguage;

        options.Clear();

        for (int i = 0; i < GameSettingDefinition.resolutionList.Count; i++)
        {
            options.Add(string.Format("{0:0} x {1:0}",
                GameSettingDefinition.resolutionList[i].x, GameSettingDefinition.resolutionList[i].y));
        }

        resolutionOptionUI.SetOptions(options);
        resolutionOptionUI.onValueChanged += ChangeResolution;

        options.Clear();

        for (int i = 0; i < GameSettingDefinition.windowModeList.Count; i++)
        {
            options.Add(string.Format(GameSettingDefinition.windowModeList[i].ToString()));
        }

        isWindowOptionUI.SetOptions(options);
        isWindowOptionUI.onValueChanged += ChangeIsWindow;

        options.Clear();

        for (int i = 0; i < GameSettingDefinition.frameList.Count; i++)
        {
            options.Add(string.Format("{0}fps",GameSettingDefinition.frameList[i]));
        }

        frameOptionUI.SetOptions(options);
        frameOptionUI.onValueChanged += ChangeFrameRate;

        options.Clear();

        for (int i = 0; i < GameSettingDefinition.vSyncList.Count; i++)
        {
            options.Add(string.Format(GameSettingDefinition.vSyncList[i].ToString()));
        }

        vSyncOptionUI.SetOptions(options);
        vSyncOptionUI.onValueChanged += ChangeVsync;

        options.Clear();

        for (int i = 0; i <= 100; i += 5)
        {
            options.Add(i.ToString());
        }

        backgroundVolumeUI.SetOptions(options);
        sfxVolumeUI.SetOptions(options);

        backgroundVolumeUI.onValueChanged += ChangeBackgroundVolume;
        sfxVolumeUI.onValueChanged += ChangeSfxVolume;

        options.Clear();
    }


    private void ChangeLanguage(int optionIndex)
    {
        LocalizationSettings.SelectedLocale = 
            LocalizationSettings.AvailableLocales.Locales[optionIndex];
    }

    private void ChangeResolution(int optionIndex)
    {
        Screen.SetResolution(
                (int)GameSettingDefinition.resolutionList[optionIndex].x,
                (int)GameSettingDefinition.resolutionList[optionIndex].y,
                GameSettingDefinition.windowModeList[isWindowOptionUI.GetCurrentIndex()]);
    }



    private void ChangeIsWindow(int optionIndex)
    {
        Screen.SetResolution(
                (int)GameSettingDefinition.resolutionList[resolutionOptionUI.GetCurrentIndex()].x,
                (int)GameSettingDefinition.resolutionList[resolutionOptionUI.GetCurrentIndex()].y,
                GameSettingDefinition.windowModeList[optionIndex]);
    }

    private void ChangeFrameRate(int optionIndex)
    {
        Application.targetFrameRate = GameSettingDefinition.frameList[optionIndex];
    }

    private void ChangeVsync(int optionIndex)
    {
        QualitySettings.vSyncCount = GameSettingDefinition.vSyncList[optionIndex] == true ? 1 : 0;
    }

    private void ChangeBackgroundVolume(int optionIndex)
    {
        SoundManager.instance.SetBackgroundVolume(int.Parse(backgroundVolumeUI.GetCurrentValue()));
    }

    private void ChangeSfxVolume(int optionIndex)
    {
        SoundManager.instance.SetSfxVolume(int.Parse(sfxVolumeUI.GetCurrentValue()));
    }



    private void SaveSettingData()
    {
        GameSettingManager.instance.gameSettingData.langaugeIndex = languageOptionUI.GetCurrentIndex();
        GameSettingManager.instance.gameSettingData.resolutionIndex = resolutionOptionUI.GetCurrentIndex();
        GameSettingManager.instance.gameSettingData.frameIndex = frameOptionUI.GetCurrentIndex();
        GameSettingManager.instance.gameSettingData.isWindowIndex = isWindowOptionUI.GetCurrentIndex();
        GameSettingManager.instance.gameSettingData.vSyncIndex = vSyncOptionUI.GetCurrentIndex();
        GameSettingManager.instance.gameSettingData.backgroundSoundVolume = int.Parse(backgroundVolumeUI.GetCurrentValue());
        GameSettingManager.instance.gameSettingData.isWindowIndex = int.Parse(sfxVolumeUI.GetCurrentValue());

        GameSettingManager.instance.gameSettingData.SaveData();
    }



    private void HideOptionUI()
    {
        SaveSettingData();
        gameObject.SetActive(false);
    }
}

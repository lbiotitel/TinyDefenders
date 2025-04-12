using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class GameSettingManager : MonoBehaviour
{
    public static GameSettingManager instance { get; private set; }
    public GameSettingSO gameSettingData { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitGameSetting();
    }

    private void InitGameSetting()
    {
        gameSettingData = Resources.Load<GameSettingSO>(Path.gameSettingDataPath);
        gameSettingData.LoadData();

        Application.runInBackground = true;
        Application.targetFrameRate = GameSettingDefinition.frameList[gameSettingData.frameIndex];
        QualitySettings.vSyncCount = GameSettingDefinition.vSyncList[gameSettingData.vSyncIndex] == true ? 1 : 0;

        Screen.SetResolution(
            (int)GameSettingDefinition.resolutionList[gameSettingData.resolutionIndex].x,
            (int)GameSettingDefinition.resolutionList[gameSettingData.resolutionIndex].y,
           GameSettingDefinition.windowModeList[gameSettingData.isWindowIndex]);

        SoundManager.instance.SetBackgroundVolume(gameSettingData.backgroundSoundVolume);
        SoundManager.instance.SetSfxVolume(gameSettingData.sfxSoundVolume);
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[gameSettingData.langaugeIndex];
    }

    public void SaveGameSetting()
    {
        gameSettingData.SaveData();
    }

    private void OnApplicationQuit()
    {
        gameSettingData.SaveData();
    }

}

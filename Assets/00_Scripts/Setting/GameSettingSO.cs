using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class GameSettingData
{
    public float backgroundSoundVolume;
    public float sfxSoundVolume;
    public int targetFrame;
    public int resolutionIndex;
    public bool isWindow;
}

public class ResolutionData
{
    public static List<Vector2> resolutionList = new List<Vector2> {
        new Vector2(1280,720),
        new Vector2(1920,1080),
        new Vector2(2560,1440),
        new Vector2(3840,1440)
    };
}


[CreateAssetMenu(fileName = "GameSettingSO", menuName = "Scriptable Objects/GameSettingSO")]
public class GameSettingSO : ScriptableObject
{
    public float backgroundSoundVolume;
    public float sfxSoundVolume;
    public int targetFrame;
    public int resolutionIndex;
    public bool isWindow;

    public void SaveData()
    {
        GameSettingData gameSettingData = new GameSettingData
        {
            backgroundSoundVolume = this.backgroundSoundVolume,
            sfxSoundVolume = this.sfxSoundVolume,
            targetFrame = this.targetFrame,
            resolutionIndex = this.resolutionIndex,
            isWindow = this.isWindow
        };

        
        string json = JsonUtility.ToJson(gameSettingData);
        try
        {
            string path = Application.persistentDataPath + "/GameSettingData.json";

            if (File.Exists(path)) File.Delete(path);
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        Debug.Log(Application.persistentDataPath + "/GameSettingData.json");

    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/GameSettingData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameSettingData data = JsonUtility.FromJson<GameSettingData>(json);
            this.backgroundSoundVolume = data.backgroundSoundVolume;
            this.sfxSoundVolume = data.sfxSoundVolume;
            this.targetFrame = data.targetFrame;
            this.resolutionIndex = data.resolutionIndex;
            this.isWindow = data.isWindow;
        }
    }



}

using System.IO;
using Unity.Services.Lobbies.Models;
using UnityEngine;


[System.Serializable]
public class GameSettingData {
    public float backgroundSoundVolume;
    public float sfxSoundVolume;
}


[CreateAssetMenu(fileName = "GameSettingSO", menuName = "Scriptable Objects/GameSettingSO")]
public class GameSettingSO : ScriptableObject
{
    public float backgroundSoundVolume;
    public float sfxSoundVolume;

    public void SaveData()
    {
        GameSettingData gameSettingData = new GameSettingData
        {
            backgroundSoundVolume = backgroundSoundVolume,
            sfxSoundVolume = this.sfxSoundVolume
        };
        string json = JsonUtility.ToJson(gameSettingData);
        File.WriteAllText(Application.persistentDataPath + "/GameSettingData.json", json);
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
        }
    }


   
}

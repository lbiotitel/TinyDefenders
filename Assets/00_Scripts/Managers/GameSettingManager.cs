using UnityEngine;

public class GameSettingManager : MonoBehaviour
{
    public static GameSettingManager instance { get; private set; }
    private GameSettingSO gameSettingData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameSettingData = Resources.Load<GameSettingSO>("SettingData/GameSettingSO");
        gameSettingData.LoadData();
        Application.targetFrameRate = gameSettingData.targetFrame;
        Application.runInBackground = true;
    }

    public void SetBackgroundVolume(float volume)
    { 
        
    }


    private void OnDestroy()
    {
        if(instance != null)
            gameSettingData.SaveData();
    }

}

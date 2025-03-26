using UnityEngine;

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
        gameSettingData = Resources.Load<GameSettingSO>("SettingData/GameSettingSO");
        gameSettingData.LoadData();

        Application.targetFrameRate = gameSettingData.targetFrame;
        Application.runInBackground = true;

        Screen.SetResolution(
            (int)ResolutionData.resolutionList[gameSettingData.resolutionIndex].x,
            (int)ResolutionData.resolutionList[gameSettingData.resolutionIndex].y,
            gameSettingData.isWindow);

    }

    public void SetBackgroundVolume(float volume)
    {


        gameSettingData.SaveData();
    }

    public void SetResolution(int index, bool isWindow)
    {
        gameSettingData.resolutionIndex = index;
        gameSettingData.isWindow = isWindow;
        gameSettingData.SaveData();
    }

    private void OnApplicationQuit()
    {
        gameSettingData.SaveData();
    }

}

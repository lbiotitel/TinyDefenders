using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using UnityEngine;


[System.Serializable]
public class UserData
{
    public string userName;
    public int userLevel;

    public UserData(string userName, int userLevel)
    {
        this.userName = userName;
        this.userLevel = userLevel;
    }
}


public class CloudManager : MonoBehaviour
{
    public static CloudManager instance = null;
    public UserData userData = null;
    private const string playerDataKey = "PlayerData";

    [SerializeField] private float saveInterval = 10f;
    private float saveTime = 0f;
    private bool isSave = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(instance);
        }
    }

    private void Update()
    {
        IntervalSaveUserData();
    }


    private void IntervalSaveUserData()
    {
        if (!isSave)
        {
            saveTime += Time.deltaTime;
            if (saveTime >= saveInterval)
            {
                saveTime = 0f;
                IntervalSaveUserData(() => { isSave = false; });
            }
        }
    }

    private async void IntervalSaveUserData(Action action = null)
    {
        isSave = true;
        await SaveUserData(userData);
        action?.Invoke();   
    }


    public async void SaveUserData()
    {
        await SaveUserData(userData);
    }

    private async Task SaveUserData(UserData userData)
    {
        try
        {
            string jsonData = JsonConvert.SerializeObject(userData);
            var data = new Dictionary<string, object> { { playerDataKey, jsonData } };
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            Debug.Log(userData.userName + "플레이어 저장 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError("플레이어 저장 실패" + e);
        }
    }

    public async Task<UserData> LoadUserData()
    {
        try
        {
            var loadedData = await CloudSaveService.Instance.Data.Player.LoadAsync(
                new HashSet<string> { playerDataKey });

            if (loadedData.TryGetValue(playerDataKey, out Item item))
            {
                string jsonString = item.Value.GetAs<string>();
                UserData userData = JsonConvert.DeserializeObject<UserData>(jsonString);
                this.userData = userData;
                return userData;
            }
            else {
                Debug.Log("저장된 플레이어가 존재하지 않음");
                MainSceneUI.instance.ShowNickNameUI();
                return this.userData = new UserData("",1);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("유저 데이터 불러오기 실패:" + e);
        }

        return null;
    }


}

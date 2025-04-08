using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.SceneManagement;




public class CloudManager : MonoBehaviour
{
    public static CloudManager instance = null;
    private UserData userData = null;
    private const string playerDataKey = "PlayerData";
    private const string leaderBoardKey = "TinyDefenceRank";

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
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        IntervalSaveUserData();
    }


    public UserData GetUserData() { return userData; }


    #region About User Basic Data Method

    private void IntervalSaveUserData()
    {
        if (SceneName.MainScene == SceneManager.GetActiveScene().name)
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
#if UNITY_EDITOR
            Debug.Log(userData.userName + "플레이어 저장 완료");
#endif
            await UpdateLeaderBoard(userData.rankingPoint);
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError("플레이어 저장 실패" + e);
#endif
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
            else
            {
#if UNITY_EDITOR
                Debug.Log("저장된 플레이어가 존재하지 않음");
#endif
                MainSceneUI.instance.ShowNickNameUI();
                this.userData = new UserData("", 0, 0);
                return this.userData;
            }
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError("유저 데이터 불러오기 실패:" + e);
#endif
        }

        return null;
    }

    #region About LeaderBoard
    private async Task UpdateLeaderBoard(int score)
    {
        try
        {
            Dictionary<string, string> metaData = new Dictionary<string, string>();
            metaData["userName"] = userData.userName;
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderBoardKey, score,
                new AddPlayerScoreOptions { Metadata = metaData });
#if UNITY_EDITOR
            Debug.Log("리더 보드 업데이트 완료");
#endif
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError("리더보드 저장 실패" + e);
#endif
        }
    }

    public async Task<RankData> GetUserLeaderBoard()
    {
        var leaderBoard = await LeaderboardsService.Instance.GetPlayerScoreAsync(leaderBoardKey,
            new GetPlayerScoreOptions { IncludeMetadata = true });

        string metaData = leaderBoard.Metadata;
        Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(metaData);

        RankData rankData = new RankData
        {
            userName = data["userName"],
            userRankPoint = leaderBoard.Score,
            userRank = leaderBoard.Rank,
            userTier = leaderBoard.Tier,
        };

        return rankData;
    }

    public async Task<List<RankData>> GetLeaderBoard()
    {
        var leaderBoard = await LeaderboardsService.Instance.GetScoresAsync(leaderBoardKey,
             new GetScoresOptions { IncludeMetadata = true });

        List<RankData> rankDatas = new List<RankData>();

        for (int i = 0; i < leaderBoard.Total; i++)
        {
            string metaData = leaderBoard.Results[i].Metadata;
            Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(metaData);

            rankDatas.Add(new RankData
            {
                userName = data["userName"],
                userRankPoint = leaderBoard.Results[i].Score,
                userRank = leaderBoard.Results[i].Rank,
                userTier = leaderBoard.Results[i].Tier,
            });
        }

        return rankDatas;
    }
    #endregion

    #endregion

    #region About User HeroData Method
    public void SetHeroCountValue(string heroName, int count)
    {
        userData.heroDataDic[heroName].heroCount += count;
    }
    #endregion

    public void SetRelicState(string relicName, bool isOn)
    {
        userData.relicDataDic[relicName] = isOn;
    }
}

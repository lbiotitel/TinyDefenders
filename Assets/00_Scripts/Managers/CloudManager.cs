using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Leaderboards;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;



[System.Serializable]
public class UserData
{
    [System.Serializable]
    public class HeroStoreData
    {

        public int heroLevel;
        public int heroCount;

        public HeroStoreData(int heroLevel, int heroCount)
        {
            this.heroLevel = heroLevel;
            this.heroCount = heroCount;
        }
    }

    [System.Serializable]
    public class SpellCardStoreData {
        public bool isOwned;

        public SpellCardStoreData(bool isOwned)
        {
            this.isOwned = isOwned;
        }
    }

    public string userName;
    public int userLevel;
    public int bestWave = 1;
    public int crystal;
    public int gold;
    public int rankingPoint;
    public Dictionary<string, HeroStoreData> heroDatas;
    public Dictionary<string, SpellCardStoreData> spellCardDatas;

    public UserData(string userName, int userLevel, int bestWave, int crystal, int gold, int rakingPoint)
    {
        this.userName = userName;
        this.userLevel = userLevel;
        this.bestWave = bestWave;
        this.crystal = crystal;
        this.gold = gold;
        this.rankingPoint = rakingPoint;
        this.heroDatas = new Dictionary<string, HeroStoreData>();
        this.spellCardDatas = new Dictionary<string, SpellCardStoreData>();

        InitHeroDatas();
        InitSpellCardDatas();
    }

    private void InitHeroDatas()
    {
        List<HeroScriptable> heroDataList = SoDataManager.instance.GetAllHeroScriptableData();

        for (int i = 0; i < heroDataList.Count; i++)
        {
            heroDatas.Add(heroDataList[i].heroName, new HeroStoreData(0, 0));
        }
    }

    private void InitSpellCardDatas()
    {
        List<CardSO> spellCardList = SoDataManager.instance.GetAllCardScriptableObjectData();

        for (int i = 0; i < spellCardList.Count; i++)
        {
            spellCardDatas.Add(spellCardList[i].cardName, new SpellCardStoreData(false));
        }
    }
}

public delegate void OnCrystalValueChage();
public delegate void OnGoldValueChange();

public class CloudManager : MonoBehaviour
{
    public static CloudManager instance = null;
    public UserData userData = null;
    private const string playerDataKey = "PlayerData";
    private const string leaderBoardKey = "TinyDefenceRank";

    [SerializeField] private float saveInterval = 10f;
    private float saveTime = 0f;
    private bool isSave = false;

    public OnCrystalValueChage onCrystalValueChange;
    public OnGoldValueChange onGoldValueChange;

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

    #region About User Asset Method

    public void CrystalChangeValue(int newValue)
    {
        userData.crystal = newValue;
        onCrystalValueChange?.Invoke();
    }

    public void GoldChangeValue(int newValue)
    {
        userData.gold = newValue;
        onGoldValueChange?.Invoke();
    }
    #endregion

    #region About User Basic Data Method

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
            await UpdateLeaderBoard(userData.rankingPoint);
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
            else
            {
                Debug.Log("저장된 플레이어가 존재하지 않음");
                MainSceneUI.instance.ShowNickNameUI();
                this.userData = new UserData("", 1, 1, 0, 0, 0);
                return this.userData;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("유저 데이터 불러오기 실패:" + e);
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
            Debug.Log("리더 보드 업데이트 완료");
        }
        catch (Exception e)
        {
            Debug.LogError("리더보드 저장 실패" + e);
        }
    }

    public async Task<RankData> GetUserLeaderBoard()
    {
        var leaderBoard = await LeaderboardsService.Instance.GetPlayerScoreAsync(leaderBoardKey,
            new GetPlayerScoreOptions { IncludeMetadata = true });

        string metaData = leaderBoard.Metadata;
        Dictionary<string,string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(metaData);

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

            rankDatas.Add(new RankData { 
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
        userData.heroDatas[heroName].heroCount += count;
    }
    #endregion
}

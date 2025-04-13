using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SoDataManager : MonoBehaviour
{
    public static SoDataManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitAllHeroScriptableObject();
            InitAllSpellCardSciprtableObject();
            InitUserExpData();
            InitRelic();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Dictionary<string, HeroScriptable> heroSciprtableDataDic;
    private Dictionary<string, CardSO> spellCardScriptableDataDic;
    private Dictionary<HeroTribe, HeroSynergySO> heroSynergySODataDic;
    private Dictionary<string, CombineSciptable> combineSODataDic;
    private Dictionary<string, InGameQuestSO> inGameQuestDic;
    private Dictionary<string, MonsterScriptable> monsterDataDic;
    private Dictionary<string, RelicSO> relicDataDic;
    private ProbabilitySetting inGameDrawHeroCardRarityProbabilitySetting;
    private UserRequireExpSO userExpData;

    #region About Player Skill Data

    private void InitRelic()
    {
        relicDataDic = new Dictionary<string, RelicSO>();

        RelicSO[] relicSOs = Resources.LoadAll<RelicSO>(Path.relicDataPath);

        foreach (var relicSO in relicSOs)
        {
            relicDataDic.Add(relicSO.relicName,Instantiate(relicSO));
        }
    }

    public List<RelicSO> GetAllRelicData()
    { 
        List<RelicSO> relicDataSOs = new List<RelicSO>();

        foreach (var data in relicDataDic)
        {
            relicDataSOs.Add(data.Value);
        }

        return relicDataSOs;
    }


    #endregion

    #region User Require Exp Data

    public void InitUserExpData()
    {
        userExpData = Resources.Load<UserRequireExpSO>(Path.userExpDataPath);
    }

    public int GetUserRequireExp(int level)
    {
        if (userExpData != null)
        {
            return userExpData.GetUserRequireExp(level);
        }

        return 0;
    }

    #endregion


    #region Draw Hero Card Probability

    public async Task<bool> InitDrawHeroCardProbability()
    {
        if (inGameDrawHeroCardRarityProbabilitySetting != null)
            Resources.UnloadAsset(inGameDrawHeroCardRarityProbabilitySetting);

        ResourceRequest resourceRequest = Resources.LoadAsync<ProbabilitySetting>(Path.inGameProbabilitySettingPath);

        await resourceRequest;

        inGameDrawHeroCardRarityProbabilitySetting = resourceRequest.asset as ProbabilitySetting;

        return true;
    }

    public HeroRarity GetIngameSummonRarity()
    {
        return inGameDrawHeroCardRarityProbabilitySetting.GetRarity();
    }

    public List<float> GetDrawHeroCardProbability()
    {
        return inGameDrawHeroCardRarityProbabilitySetting.GetDrawHeroCardProbability();
    }

    public void UpgradeIngameSummonRarity(float amount)
    {
        inGameDrawHeroCardRarityProbabilitySetting.AddProbability(amount);
    }

    #endregion


    #region About InGame Quest Data

    public async Task<bool> InitInGameQuestData()
    {
        inGameQuestDic = new Dictionary<string, InGameQuestSO>();

        InGameQuestSO[] inGameQuestSOs = Resources.LoadAll<InGameQuestSO>(Path.questDataPath);

        await Task.Run(() =>
        {
            for (int i = 0; i < inGameQuestSOs.Length; i++)
            {
                inGameQuestDic.Add(inGameQuestSOs[i].questName, inGameQuestSOs[i]);
            }
        });

        return true;
    }

    public List<InGameQuestSO> GetAllInGameQuest()
    {
        List<InGameQuestSO> inGameQuestSOs = new List<InGameQuestSO>();

        foreach (var data in inGameQuestDic)
        {
            inGameQuestSOs.Add(data.Value);
        }

        return inGameQuestSOs;
    }


    #endregion


    #region About Combine Data

    public async Task<bool> InitAllCombineData()
    {
        combineSODataDic = new Dictionary<string, CombineSciptable>();

        List<CombineSciptable> combineDatas = new List<CombineSciptable>();

        for (int rarity = (int)HeroRarity.Uncommon; rarity <= (int)HeroRarity.Legend; rarity++)
        {
            string rare = ((HeroRarity)rarity).ToString();
            CombineSciptable[] combineData = Resources.LoadAll<CombineSciptable>($"02CombineHeroData/{rare}");
            foreach (var data in combineData)
            {
                combineDatas.Add(data);
            }
        }

        await Task.Run(() =>
        {
            for (int i = 0; i < combineDatas.Count; i++)
            {
                combineSODataDic.Add(combineDatas[i].mainHeroData.heroName, combineDatas[i]);
            }
        });

        return true;
    }

    public List<CombineSciptable> GetAllCombineData()
    {
        List<CombineSciptable> combineList = new List<CombineSciptable>();

        foreach (var data in combineSODataDic)
        {
            combineList.Add(data.Value);
        }

        return combineList;
    }



    #endregion

    #region About Monster Data

    public async Task<bool> InitAllMonsterData(string mapName)
    {
        if (monsterDataDic != null)
        {
            foreach (var data in monsterDataDic)
            {
                Resources.UnloadAsset(data.Value);
            }
            monsterDataDic.Clear();
        }

        monsterDataDic = new Dictionary<string, MonsterScriptable>();

        MonsterScriptable[] monsterDatas = Resources.LoadAll<MonsterScriptable>(Path.monsterPath + mapName);

        await Task.Run(() => {
            foreach (var data in monsterDatas)
            {
                monsterDataDic.Add(data.monsterName, data);
            }
        });

        return true;
    }

    public MonsterScriptable GetMonsterData(string monsterName)
    {
        if (monsterDataDic.ContainsKey(monsterName))
        { 
            return monsterDataDic[monsterName];
        }

        return null;
    }


    #endregion


    #region About Hero Synergy Data 
    public async Task<bool> InitAllHeroSynergyData()
    {
        heroSynergySODataDic = new Dictionary<HeroTribe, HeroSynergySO>();

        HeroSynergySO[] heroSynergySOs = Resources.LoadAll<HeroSynergySO>(Path.heroSynergyDataPath);

        await Task.Run(() =>
        {
            foreach (var heroSynergySO in heroSynergySOs)
            {
                heroSynergySODataDic.Add(heroSynergySO.synergyTribe, heroSynergySO);
            }
        });

        return true;
    }

    public HeroSynergySO GetHeroSynergySO(HeroTribe synergyTribe)
    {
        if (heroSynergySODataDic.ContainsKey(synergyTribe))
        {
            return heroSynergySODataDic[synergyTribe];
        }

        return null;
    }

    public List<HeroSynergySO> GetHeroSynergySOs()
    {
        List<HeroSynergySO> heroSynergySOs = new List<HeroSynergySO>();

        foreach (var data in heroSynergySODataDic)
        {
            heroSynergySOs.Add(data.Value);
        }

        return heroSynergySOs;
    }
    #endregion


    #region About Hero So Resource Method
    private void InitAllHeroScriptableObject()
    {
        heroSciprtableDataDic = new Dictionary<string, HeroScriptable>();

        for (int rarity = 0; rarity <= (int)HeroRarity.Legend; rarity++)
        {
            string rare = ((HeroRarity)rarity).ToString();
            HeroScriptable[] datas = Resources.LoadAll<HeroScriptable>($"01HeroDatas/{rare}");

            for (int heroIndex = 0; heroIndex < datas.Length; heroIndex++)
            {
                heroSciprtableDataDic.Add(datas[heroIndex].heroName, Instantiate(datas[heroIndex]));
            }
        }
    }

    public HeroScriptable GetHeroScriptableData(string heroName)
    {
        if (heroSciprtableDataDic.ContainsKey(heroName))
        {
            return heroSciprtableDataDic[heroName];
        }

        return null;
    }


    public List<HeroScriptable> GetAllHeroScriptableData()
    {
        List<HeroScriptable> heroDatas = new List<HeroScriptable>();

        foreach (var data in heroSciprtableDataDic)
        {
            heroDatas.Add(data.Value);
        }

        return heroDatas;
    }


    //테스트를 위해 normal로 고정
    public HeroScriptable GetRandomHeroScriptiableData()
    {
        HeroRarity heroRarity = GetIngameSummonRarity();

        List<HeroScriptable> heroDatas = GetAllHeroScriptableData();
        List<HeroScriptable> heroDatasByRarity = new List<HeroScriptable>();

        foreach (var data in heroDatas)
        {
            if (data.rarity == heroRarity)
                heroDatasByRarity.Add(data);
        }

        int selectHero = Random.Range(0, heroDatasByRarity.Count);

        return heroDatasByRarity[selectHero];
    }
    #endregion


    #region About SpellCard Resource Method

    private void InitAllSpellCardSciprtableObject()
    {
        spellCardScriptableDataDic = new Dictionary<string, CardSO>();

        for (int rarity = 0; rarity <= (int)CardRarity.Rare; rarity++)
        {
            string rare = ((CardRarity)rarity).ToString();
            CardSO[] datas = Resources.LoadAll<CardSO>($"04Card/{rare}");

            for (int cardIndex = 0; cardIndex < datas.Length; cardIndex++)
            {
                spellCardScriptableDataDic.Add(datas[cardIndex].cardName, datas[cardIndex]);
            }
        }
    }

    public CardSO GetSpellCardScirptableObject(string spellName)
    {
        if (spellCardScriptableDataDic.ContainsKey(spellName))
        {
            return spellCardScriptableDataDic[spellName];
        }

        return null;
    }

    public List<CardSO> GetAllSpellCardScirptableObjectByRarity(CardRarity rarity)
    {
        List<CardSO> rarityCardSO = new List<CardSO>();

        foreach (var data in spellCardScriptableDataDic)
        {
            if (data.Value.cardRarity == rarity)
            {
                rarityCardSO.Add(data.Value);
            }
        }

        return rarityCardSO;
    }

    public List<CardSO> GetAllCardScriptableObjectData()
    {
        List<CardSO> cardDatas = new List<CardSO>();

        foreach (var data in spellCardScriptableDataDic)
        {
            cardDatas.Add(data.Value);
        }


        return cardDatas;
    }

    #endregion

    private void OnApplicationQuit()
    {
        if (instance != null)
        {
            Resources.UnloadUnusedAssets();
            instance = null;
        }
    }
}

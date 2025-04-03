using System.Collections.Generic;
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
            InitAllHeroSynergyData();
            InitAllCombineData();
            InitDrawHeroCardProbability();
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private Dictionary<string, HeroScriptable> heroSciprtableDataDic;
    private Dictionary<string, CardSO> spellCardScriptableDataDic;
    private Dictionary<HeroTribe, HeroSynergySO> heroSynergySODataDic;
    private Dictionary<string, CombineSciptable> combineSODataDic;
    private ProbabilitySetting inGameDrawHeroCardRarityProbabilitySetting;


    #region Draw Hero Card Probability

    private void InitDrawHeroCardProbability()
    {
        inGameDrawHeroCardRarityProbabilitySetting = Instantiate(Resources.Load<ProbabilitySetting>("SettingData/InGameProbabilitySettingData"));
    }

    public HeroRarity GetIngameSummonRarity()
    {
        return inGameDrawHeroCardRarityProbabilitySetting.GetRarity();
    }

    public List<float> GetDrawHeroCardProbability()
    {
        return inGameDrawHeroCardRarityProbabilitySetting.GetDrawHeroCardProbability();
    }

    public void UpgradeIngameSummonRarity(int level)
    {
        inGameDrawHeroCardRarityProbabilitySetting.AddProbability(level);
    }



    #endregion

    #region About Combine Data

    private void InitAllCombineData()
    {
        combineSODataDic = new Dictionary<string, CombineSciptable>();

        for (int rarity = (int)HeroRarity.Uncommon; rarity <= (int)HeroRarity.Legend; rarity++)
        {
            string rare = ((HeroRarity)rarity).ToString();
            CombineSciptable[] combineDatas = Resources.LoadAll<CombineSciptable>($"CombineHeroData/{rare}");

            for (int i = 0; i < combineDatas.Length; i++)
            {
                combineSODataDic.Add(combineDatas[i].mainHeroData.heroName, combineDatas[i]);
            }
        }
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


    #region About Hero Synergy Data 
    private void InitAllHeroSynergyData()
    {
        heroSynergySODataDic = new Dictionary<HeroTribe, HeroSynergySO>();

        HeroSynergySO[] heroSynergySOs = Resources.LoadAll<HeroSynergySO>("HeroSynergyDatas");

#if UnityEidtor
        Debug.Log(heroSynergySOs.Length);
#endif
        for(int i = 0; i < heroSynergySOs.Length; i++)
        {
            heroSynergySODataDic.Add(heroSynergySOs[i].synergyTribe, heroSynergySOs[i]);
        }   
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
            HeroScriptable[] datas = Resources.LoadAll<HeroScriptable>($"HeroDatas/{rare}");

            for (int heroIndex = 0; heroIndex < datas.Length; heroIndex++)
            {
                heroSciprtableDataDic.Add(datas[heroIndex].heroName, datas[heroIndex]);
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
            if(data.rarity == heroRarity)
                heroDatasByRarity.Add(data);
        }

        int selectHero = Random.Range(0,heroDatasByRarity.Count);

        return heroDatasByRarity[selectHero];
    }
    #endregion


    #region About SpellCard Resource Method

    private void InitAllSpellCardSciprtableObject()
    {
        spellCardScriptableDataDic = new Dictionary<string,  CardSO>();

        for (int rarity = 0; rarity <= (int)CardRarity.Rare; rarity++)
        {
            string rare = ((CardRarity)rarity).ToString();
            CardSO[] datas = Resources.LoadAll<CardSO>($"Card/{rare}");

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

}

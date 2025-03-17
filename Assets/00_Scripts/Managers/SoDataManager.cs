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
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private Dictionary<string, HeroScriptable> heroSciprtableDatas;
    private Dictionary<string, CardSO> spellCardScriptableDatas;


    #region About Hero So Resource Method
    private void InitAllHeroScriptableObject()
    {
        heroSciprtableDatas = new Dictionary<string, HeroScriptable>();

        for (int rarity = 0; rarity <= (int)HeroRarity.God; rarity++)
        {
            string rare = ((HeroRarity)rarity).ToString();
            HeroScriptable[] datas = Resources.LoadAll<HeroScriptable>($"HeroDatas/{rare}");

            for (int heroIndex = 0; heroIndex < datas.Length; heroIndex++)
            {
                heroSciprtableDatas.Add(datas[heroIndex].heroName, datas[heroIndex]);
            }
        }
    }

    public HeroScriptable GetHeroScriptableData(string heroName)
    {
        if (heroSciprtableDatas.ContainsKey(heroName))
        {
            return heroSciprtableDatas[heroName];
        }

        return null;
    }


    public List<HeroScriptable> GetAllHeroScriptableData()
    {
        List<HeroScriptable> heroDatas = new List<HeroScriptable>();

        foreach (var data in heroSciprtableDatas)
        {
            heroDatas.Add(data.Value);
        }

        return heroDatas;
    }
    #endregion


    #region About SpellCard Resource Method

    private void InitAllSpellCardSciprtableObject()
    {
        spellCardScriptableDatas = new Dictionary<string,  CardSO>();

        for (int rarity = 0; rarity <= (int)CardRarity.Rare; rarity++)
        {
            string rare = ((CardRarity)rarity).ToString();
            CardSO[] datas = Resources.LoadAll<CardSO>($"Card/{rare}");

            for (int cardIndex = 0; cardIndex < datas.Length; cardIndex++)
            {
                spellCardScriptableDatas.Add(datas[cardIndex].cardName, datas[cardIndex]);
            }
        }
    }

    public CardSO GetSpellCardScirptableObject(string spellName)
    {
        if (spellCardScriptableDatas.ContainsKey(spellName))
        { 
            return spellCardScriptableDatas[spellName];
        }

        return null;
    }

    public List<CardSO> GetAllCardScriptableData()
    {
        List<CardSO> cardDatas = new List<CardSO>();

        foreach (var data in spellCardScriptableDatas)
        {
            cardDatas.Add(data.Value);
        }


        return cardDatas;
    }

    #endregion

}

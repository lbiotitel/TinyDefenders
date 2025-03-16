using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using static UserData;


public class Utils : MonoBehaviour
{
    private static ProbabilitySetting shopProbabilitySetting = Resources.Load<ProbabilitySetting>("SettingData/ShopProbabilitySettingData");
    private static ProbabilitySetting inGameProbabilitySetting = Resources.Load<ProbabilitySetting>("SettingData/InGameProbabilitySettingData");
    private static Dictionary<string, HeroScriptable> heroSciprtableDatas;


    public static void InitAllHeroScriptableObject()
    {
        heroSciprtableDatas = new Dictionary<string, HeroScriptable>();

        for (int rarity = 0; rarity <= (int)Rarity.God; rarity++)
        {
            string rare = ((Rarity)rarity).ToString();
            HeroScriptable[] datas = Resources.LoadAll<HeroScriptable>($"HeroDatas/{rare}");

            for (int heroIndex = 0; heroIndex < datas.Length; heroIndex++)
            {
                heroSciprtableDatas.Add(datas[heroIndex].heroName, datas[heroIndex]);
            }
        }
    }

    public static HeroScriptable GetHeroScriptableData(string heroName)
    {
        if (heroSciprtableDatas.ContainsKey(heroName))
        { 
            return heroSciprtableDatas[heroName];
        }

        return null;
    }


    public static List<HeroScriptable> GetHeroScriptableAllDatas()
    {
        List<HeroScriptable> heroDatas = new List<HeroScriptable>();

        foreach (var data in heroSciprtableDatas)
        {
            heroDatas.Add(data.Value);
        }

        Debug.Log(heroSciprtableDatas);

        return heroDatas;
    }

    public static Rarity GetShopSummonRarity()
    {
        return shopProbabilitySetting.GetRarity();
    }

    public static Rarity GetIngameSummonRarity()
    { 
        return inGameProbabilitySetting.GetRarity();
    }


    public static Color GetRarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Normal: return new Color(0.69f, 0.69f, 0.69f, 0.7f);
            case Rarity.Uncommon: return new Color(0.16f, 0.47f, 1.0f, 0.7f);
            case Rarity.Rare: return new Color(0.56f, 0.14f, 0.66f, 0.7f);
            case Rarity.Legend: return new Color(1.0f, 0.84f, 0.0f, 0.7f);
            case Rarity.God: return new Color(0.0f, 0.78f, 0.33f, 0.7f);
        }

        return Color.white;
    }


    public static string Localization_Text(Localize localize, string key)
    {
        Locale currentLanguage = LocalizationSettings.SelectedLocale;
        string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(localize.ToString(), key, currentLanguage);
        return localizedString;
    }
}

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



    public static HeroRarity GetShopSummonRarity()
    {
        return shopProbabilitySetting.GetRarity();
    }

    public static HeroRarity GetIngameSummonRarity()
    { 
        return inGameProbabilitySetting.GetRarity();
    }


    public static Color GetRarityColor(HeroRarity rarity)
    {
        switch (rarity)
        {
            case HeroRarity.Normal: return new Color(0.69f, 0.69f, 0.69f, 0.7f);
            case HeroRarity.Uncommon: return new Color(0.16f, 0.47f, 1.0f, 0.7f);
            case HeroRarity.Rare: return new Color(0.56f, 0.14f, 0.66f, 0.7f);
            case HeroRarity.Legend: return new Color(1.0f, 0.84f, 0.0f, 0.7f);
            case HeroRarity.God: return new Color(0.0f, 0.78f, 0.33f, 0.7f);
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

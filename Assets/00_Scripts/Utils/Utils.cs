using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;


public class Utils : MonoBehaviour
{
    private static ProbabilitySetting probabilitySetting = Resources.Load<ProbabilitySetting>("SettingData/ProbabilitySettingData");


    public static Rarity GetSummonRarity()
    { 
        return probabilitySetting.GetRarity();
    }

    public static string Localization_Text(Localize localize, string key)
    {
        Locale currentLanguage = LocalizationSettings.SelectedLocale;
        string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(localize.ToString(), key, currentLanguage);
        return localizedString;
    }
}

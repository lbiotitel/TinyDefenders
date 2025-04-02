using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using static UserData;


public class Utils : MonoBehaviour
{
    private static ProbabilitySetting shopProbabilitySetting = Resources.Load<ProbabilitySetting>("SettingData/ShopProbabilitySettingData");

    public static HeroRarity GetShopSummonRarity()
    {
        return shopProbabilitySetting.GetRarity();
    }


    public static Color GetRarityColor(HeroRarity rarity)
    {
        switch (rarity)
        {
            case HeroRarity.Normal: return new Color(0.69f, 0.69f, 0.69f, 0.7f);
            case HeroRarity.Uncommon: return new Color(0.16f, 0.47f, 1.0f, 0.7f);
            case HeroRarity.Rare: return new Color(0.56f, 0.14f, 0.66f, 0.7f);
            case HeroRarity.Legend: return new Color(1.0f, 0.84f, 0.0f, 0.7f);
        }

        return Color.white;
    }

    public static void ModifyTransform(Transform targetTransform, ref RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector3 newPosition = targetTransform.position;
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

        if (!screenRect.Contains(Camera.main.WorldToScreenPoint(corners[0])))
        {
            newPosition.x = Mathf.Max(newPosition.x, rectTransform.rect.width / 2f);
            newPosition.y = Mathf.Max(newPosition.y, rectTransform.rect.height / 2f);
        }
        if (!screenRect.Contains(Camera.main.WorldToScreenPoint(corners[2])))
        {
            newPosition.x = Mathf.Min(newPosition.x, Screen.width - rectTransform.rect.width / 2f);
            newPosition.y = Mathf.Min(newPosition.y, Screen.height - rectTransform.rect.height / 2f);
        }

        rectTransform.position = newPosition;
    }



    public static string Localization_Text(Localize localize, string key)
    {
        Locale currentLanguage = LocalizationSettings.SelectedLocale;
        string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(localize.ToString(), key, currentLanguage);
        return localizedString;
    }
}

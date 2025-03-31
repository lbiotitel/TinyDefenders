using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombineHeroInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI heroNameText;
    [SerializeField] private TextMeshProUGUI heroRarityText;
    [SerializeField] private Image heroImage;
    [SerializeField] private TextMeshProUGUI heroDescriptionText;
    [SerializeField] private TextMeshProUGUI heroAtkText;
    [SerializeField] private TextMeshProUGUI heroAtkSpeedText;
    [SerializeField] private TextMeshProUGUI heroAtkRangeText;
    [SerializeField] private TextMeshProUGUI heroCriticalRateText;
    [SerializeField] private float yOffset;
    [SerializeField] private List<Image> subHeroImageList;
    private RectTransform rectTransform;

    private void Awake()
    {
        HideCombineHeroInfoUI();
        rectTransform = GetComponent<RectTransform>();
    }

    public void ShowCombineHeroInfoUI(HeroScriptable heroData,List<HeroScriptable> subHeroList, Transform targetTransform)
    {
        heroNameText.text = heroData.heroName;
        heroRarityText.text = heroData.rarity.ToString();
        heroImage.sprite = heroData.heroMainImage;
        heroDescriptionText.text = heroData.GetLocalizationDesc();
        heroAtkText.text = heroData.atk.baseValue.ToString();
        heroAtkSpeedText.text = heroData.atkSpeed.baseValue.ToString();
        heroAtkRangeText.text = heroData.atkRange.baseValue.ToString();
        heroCriticalRateText.text = heroData.criticalRate.baseValue.ToString() + "%";
        ShowSubHeroImageList(subHeroList);

        Utils.ModifyTransform(targetTransform,ref rectTransform);

        rectTransform.transform.position += new Vector3(0, yOffset, 0);

        gameObject.SetActive(true);
    }

    private void ShowSubHeroImageList(List<HeroScriptable> subHeroList)
    {
        foreach (var subHeroImage in subHeroImageList)
        {
            subHeroImage.gameObject.SetActive(false);
        }

        int imageIndex = 0;

        for (int i = 0; i < subHeroList.Count; i++)
        {
            subHeroImageList[imageIndex].sprite = subHeroList[i].heroMainImage;
            subHeroImageList[imageIndex].gameObject.SetActive(true);
            imageIndex++;
        }
    }



    public void HideCombineHeroInfoUI()
    {
        gameObject.SetActive(false);
    }
}

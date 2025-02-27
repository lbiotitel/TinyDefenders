using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoUI : MonoBehaviour
{
    [SerializeField] private Image heroImage;
    [SerializeField] private TextMeshProUGUI heroNameText;
    [SerializeField] private TextMeshProUGUI heroRarity;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI attackRangeText;
    [SerializeField] private TextMeshProUGUI criticalRateText;

    public void ShowHeroInfoUI(HeroScriptable heroData)
    {
        gameObject.SetActive(true);
        heroImage.sprite = heroData.heroMainImage;
        heroNameText.text = heroData.heroName;
        heroRarity.text = heroData.rarity.ToString();
        attackText.text = heroData.atk.ToString();
        attackSpeedText.text = heroData.atkSpeed.ToString();
        attackRangeText.text = heroData.atkRange.ToString(); 
    }

    public void HideHeroInfoUI()
    {
        gameObject.SetActive(false);
    }

}

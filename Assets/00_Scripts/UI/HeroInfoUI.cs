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

    [SerializeField] private Button sellButton;
    [SerializeField] private Button compositionButton;

    private Hero hero;

    private void Start()
    {
        InitButtons();
    }

    private void InitButtons()
    {
        sellButton.onClick.AddListener(() =>
        {
            if (this.hero != null)
            {
                hero.GetHeroHolder().Sell(true);
            }
        });

        compositionButton.onClick.AddListener(() => {
            if (this.hero != null)
            {
                hero.GetHeroHolder().Composition();
            }
        });
    }

    

    public void ShowHeroInfoUI(Hero hero)
    {
        gameObject.SetActive(true);
        HeroScriptable heroData = hero.GetHeroData();
        this.hero = hero;
        this.heroImage.sprite = heroData.heroMainImage;
        this.heroNameText.text = heroData.heroName;
        this.heroRarity.text = heroData.rarity.ToString();
        this.attackText.text = heroData.atk.ToString();
        this.attackSpeedText.text = heroData.atkSpeed.ToString();
        this.attackRangeText.text = heroData.atkRange.ToString(); 
    }

    public void HideHeroInfoUI()
    {
        this.hero = null;
        gameObject.SetActive(false);
    }

}

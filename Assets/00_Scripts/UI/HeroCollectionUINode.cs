using Coffee.UIExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroCollectionUINode : MonoBehaviour
{ 

    [SerializeField] protected Image backgroundCardImage;
    [SerializeField] protected TextMeshProUGUI cardNameText;
    [SerializeField] protected TextMeshProUGUI cardDescText;
    [SerializeField] protected TextMeshProUGUI cardCostText;
    [SerializeField] protected Image heroImage;

    [SerializeField] protected CardColorNode[] cardColorNode;

    [SerializeField] protected TextMeshProUGUI heroAtkText;
    [SerializeField] protected TextMeshProUGUI heroAtkSpeedText;
    [SerializeField] protected TextMeshProUGUI heroAtkRangeText;

    [SerializeField] protected Image tribeImage;
    [SerializeField] protected Image classImage;


    public void Init(HeroScriptable heroData)
    {
        heroImage.sprite = heroData.heroMainImage;

        heroAtkText.text = heroData.atk.baseValue.ToString();
        heroAtkSpeedText.text = heroData.atkSpeed.baseValue.ToString();
        heroAtkRangeText.text = heroData.atkRange.baseValue.ToString();

        cardCostText.text = heroData.heroCost.ToString();
        cardNameText.text = heroData.GetLocalizationName();
        cardDescText.text = heroData.heroDesc;

        tribeImage.sprite = GameResourceManager.instance.GetTribeImage(heroData.heroTribe);
        classImage.sprite = GameResourceManager.instance.GetClassImage(heroData.heroClass);

        InitCardRarityBackgroundImage((CardRarity)((int)heroData.rarity));
    }

    protected void InitCardRarityBackgroundImage(CardRarity cardRarity)
    {
        backgroundCardImage.sprite = cardColorNode[(int)cardRarity].cardImage;
    }

}

using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroCardNode : CardNodeBase
{
    [System.Serializable]
    public class HeroTribeNode {
        public HeroTribe tribe;
        public Sprite heroTribeImage;
    }

    [Header("HeroCard Info")]
    [SerializeField] protected TextMeshProUGUI heroAtkText;
    [SerializeField] protected TextMeshProUGUI heroAtkSpeedText;
    [SerializeField] protected TextMeshProUGUI heroAtkRangeText;
    [SerializeField] protected Image tribeImage;
    [SerializeField] private HeroTribeNode[] heroTribeNodes;

    private HeroCard summonCard;

    public override void InitCardNode(CardSO cardSO)
    {
        base.InitCardNode(cardSO);

        summonCard = cardSO as HeroCard;

        HeroScriptable heroData = summonCard.GetHeroData();

        cardImage.sprite = heroData.heroMainImage;

        heroAtkText.text = heroData.atk.baseValue.ToString();
        heroAtkSpeedText.text = heroData.atkSpeed.baseValue.ToString();
        heroAtkRangeText.text = heroData.atkRange.baseValue.ToString();

        cardCostText.text = heroData.heroCost.ToString();
        cardNameText.text = heroData.GetLocalizationName();
        cardDescText.text = heroData.heroDesc;
        tribeImage.sprite = GetTribeImage(heroData.heroTribe);

        InitCardRarityBackgroundImage((CardRarity)((int)heroData.rarity));
    }

    private Sprite GetTribeImage(HeroTribe heroTribe)
    {
        for (int i = 0; i < heroTribeNodes.Length; i++)
        {
            if (heroTribeNodes[i].tribe == heroTribe)
            {
                return heroTribeNodes[i].heroTribeImage;
            }
        }

        return null;
    }

    protected override void UseCard()
    {
        if (SpawnManager.instance.CanSpawn(NetUtils.GetPlayerNetworkType()))
        {
            summonCard.Summon(transform);
            base.UseCard();
        }
        else {
            ResetCardTransform();
        }
    }

}

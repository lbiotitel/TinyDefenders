using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoUI : MonoBehaviour
{
    [SerializeField] private Image heroImage;
    [SerializeField] private TMP_Text heroNameText;
    [SerializeField] private TMP_Text heroRarity;
    [SerializeField] private TMP_Text heroDesc;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text attkackBounusText;
    [SerializeField] private TMP_Text attackSpeedText;
    [SerializeField] private TMP_Text attackSpeedBounusText;
    [SerializeField] private TMP_Text attackRangeText;
    [SerializeField] private TMP_Text attackRangeBonusText;
    [SerializeField] private TMP_Text criRateText;
    [SerializeField] private TMP_Text criRateBounusText;
    [SerializeField] private TMP_Text monsterDamageStack;
    [SerializeField] private TMP_Text sellCostText;
    [SerializeField] private Button sellButton;

    [SerializeField] private GameObject parentObject;
    [SerializeField] private Animator animator;

    private Hero hero;

    private void Start()
    {
        InitButtons();
        HideHeroInfoUI();
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            ShowHeroInfoUI(hero);
        }
    }

    private void InitButtons()
    {
        sellButton.onClick.AddListener(() =>
        {
            if (this.hero != null)
            {
                hero.GetHeroHolder().Sell();
                HideHeroInfoUI();
            }
        });
    }

    public void ShowHeroInfoUI(Hero hero)
    {
        if (hero == null) return;

        HeroScriptable heroData = hero.GetHeroData();

        if (!NetUtils.IsHost())
            NetworkRpcManager.instance.GetServerHeroDamageStackServerRpc(hero.NetworkObjectId);

        parentObject.SetActive(true);


        this.hero = hero;

        Debug.Log(heroData.GetHeroBaseAtkRange());
        Debug.Log(heroData.GetHeroAtkSpeed());

        this.heroImage.sprite = heroData.heroMainImage;
        this.heroNameText.text = heroData.GetLocalizationName();
        this.heroDesc.text = heroData.GetLocalizationDesc();
        this.heroRarity.text = heroData.rarity.ToString();

        this.attackText.text = string.Format("{0:N0}", heroData.GetHeroBaseAtk());
        if (heroData.GetHeroBonusAtk() != 0) this.attkackBounusText.text = string.Format("<color=#00FF00>(+{0:N0})", heroData.GetHeroBonusAtk());
        else this.attkackBounusText.text = string.Empty;

        this.attackSpeedText.text = string.Format("{0:F2}", heroData.GetHeroBaseAtkSpeed());
        if (heroData.GetHeroBounusAtkSpeed() != 0) this.attackSpeedBounusText.text = string.Format("<color=#00FF00>(+{0:F2})", heroData.GetHeroBounusAtkSpeed());
        else this.attackSpeedBounusText.text = string.Empty;

        this.attackRangeText.text = string.Format("{0:F2}", heroData.GetHeroBaseAtkRange());
        if (heroData.GetHeroBonusAtkRange() != 0) this.attackRangeBonusText.text = string.Format("<color=#00FF00>(+{0:F2})", heroData.GetHeroBonusAtkRange());
        else this.attackRangeBonusText.text = string.Empty;

        this.criRateText.text = string.Format("{0:0}%", heroData.GetHeroBaseCriticalRate());
        if (heroData.GetHeroBonusCriticalRate() != 0) this.criRateBounusText.text = string.Format("<color=#00FF00>(+{0:0})%", heroData.GetHeroBonusCriticalRate());
        else this.criRateBounusText.text = string.Empty;

        this.monsterDamageStack.text = string.Format("가한 데미지 : {0:N0}", hero.GetDamageStack());
        this.sellCostText.text = string.Format("판매 가격 : {0:N0}", heroData.heroCost / 2);
    }

    public void HideHeroInfoUI()
    {
        if (!parentObject.activeSelf) return;
        this.hero = null;
        animator.SetTrigger("HIDE");
    }

    private void OnDestroy()
    {
        sellButton.onClick.RemoveAllListeners();
    }
}

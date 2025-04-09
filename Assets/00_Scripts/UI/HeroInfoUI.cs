using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoUI : MonoBehaviour
{
    [SerializeField] private Image heroImage;
    [SerializeField] private TextMeshProUGUI heroNameText;
    [SerializeField] private TextMeshProUGUI heroRarity;
    [SerializeField] private TextMeshProUGUI heroDesc;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI attkackBounusText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI attackSpeedBounusText;
    [SerializeField] private TextMeshProUGUI attackRangeText;
    [SerializeField] private TextMeshProUGUI attackRangeBonusText;
    [SerializeField] private TextMeshProUGUI criRateText;
    [SerializeField] private TextMeshProUGUI criRateBounusText;
    [SerializeField] private TextMeshProUGUI monsterDamageStack;
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

        this.heroImage.sprite = heroData.heroMainImage;
        this.heroNameText.text = heroData.GetLocalizationName();
        this.heroDesc.text = heroData.GetLocalizationDesc();
        this.heroRarity.text = heroData.rarity.ToString();

        this.attackText.text = string.Format("{0:N0}", heroData.GetHeroBaseAtk());
        if (heroData.GetHeroBonusAtk() != 0) this.attkackBounusText.text = string.Format("<color=#00FF00>(+{0:N0})", heroData.GetHeroBonusAtk());
        else this.attkackBounusText.text = string.Empty;

        this.attackSpeedText.text = string.Format("{0:2}", heroData.GetHeroBaseAtkSpeed());
        if (heroData.GetHeroBounusAtkSpeed() != 0) this.attackSpeedBounusText.text = string.Format("<color=#00FF00>(+{0:F2})", heroData.GetHeroBounusAtkSpeed());
        else this.attackSpeedBounusText.text = string.Empty;

        this.attackRangeText.text = string.Format("{0:2}", heroData.GetHeroBaseAtkRange());
        if (heroData.GetHeroBonusAtkRange() != 0) this.attackRangeBonusText.text = string.Format("<color=#00FF00>(+{0:F2})", heroData.GetHeroBonusAtkRange());
        else this.attackRangeBonusText.text = string.Empty;

        this.criRateText.text = string.Format("{0:0}%", heroData.GetHeroBaseCriticalRate());
        if (heroData.GetHeroBonusCriticalRate() != 0) this.criRateBounusText.text = string.Format("<color=#00FF00>(+{0:0})%", heroData.GetHeroBonusCriticalRate());
        else this.criRateBounusText.text = string.Empty;

        this.monsterDamageStack.text = string.Format("가한 데미지 : {0:N0}", hero.GetDamageStack());
    }

    public void HideHeroInfoUI()
    {
        if (!parentObject.activeSelf) return;
        this.hero = null;
        animator.SetTrigger("HIDE");
    }

}

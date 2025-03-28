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
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI attackRangeText;
    [SerializeField] private TextMeshProUGUI criRateText;
    [SerializeField] private TextMeshProUGUI monsterDamageStack;

    [SerializeField] private float yPos;
    [SerializeField] private Button sellButton;

    private Hero hero;

    private void Start()
    {
        InitButtons();
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

        transform.position = hero.transform.position + new Vector3(0,yPos,0);

        HeroScriptable heroData = hero.GetHeroData();

        if (!NetUtils.IsServer())
            NetworkRpcManager.instance.GetServerHeroDamageStackServerRpc(hero.NetworkObjectId);

        gameObject.SetActive(true);


        this.hero = hero;

        this.heroImage.sprite = heroData.heroMainImage;
        this.heroNameText.text = heroData.GetLocalizationName();
        this.heroDesc.text = heroData.GetLocalizationDesc();
        this.heroRarity.text = heroData.rarity.ToString();

        this.attackText.text =
            hero.GetBonusHeroAtk() == 0 ?
            string.Format("{0:N0}", hero.GetHeroAtk()) :
            string.Format("{0:N0}<color=#00FF00>(+{1:N0})", hero.GetHeroAtk(), hero.GetBonusHeroAtk());

        this.attackSpeedText.text =
           hero.GetBounusHeroAtkSpeed() == 0 ?
            string.Format("{0:0}", hero.GetHeroAtkSpeed()) :
            string.Format("{0:0}<color=#00FF00>(+{1:0})", hero.GetHeroAtkSpeed(), hero.GetBounusHeroAtkSpeed());

        this.attackRangeText.text =
            hero.GetHeroBonusAtkRange() == 0 ?
            string.Format("{0:0}", hero.GetHeroAtkRange()) :
            string.Format("{0:0}<color=#00FF00>(+{1:0})", hero.GetHeroAtkRange(), hero.GetHeroBonusAtkRange());

        this.criRateText.text = heroData.criticalRate.baseValue.ToString();

        this.monsterDamageStack.text = string.Format("가한 데미지 : {0:N0}", hero.GetDamageStack());
    }

    public void HideHeroInfoUI()
    {
        this.hero = null;
        gameObject.SetActive(false);
    }

}

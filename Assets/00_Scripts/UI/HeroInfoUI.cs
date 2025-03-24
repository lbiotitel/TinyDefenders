using TMPro;
using Unity.Netcode;
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
    [SerializeField] private TextMeshProUGUI monsterKillCountText;


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

        compositionButton.onClick.AddListener(() =>
        {
            if (this.hero != null)
            {
                hero.GetHeroHolder().Composition();
            }
        });
    }

    public void ShowHeroInfoUI(Hero hero)
    {

        HeroScriptable heroData = hero.GetHeroData();

        if(!NetUtils.IsServer())
            NetworkRpcManager.instance.GetServerHeroKillCountServerRpc(hero.NetworkObjectId);

        gameObject.SetActive(true);


        this.hero = hero;
        this.heroImage.sprite = heroData.heroMainImage;
        this.heroNameText.text = heroData.GetLocalizationName();
        this.heroDesc.text = heroData.GetLocalizationDesc();
        this.heroRarity.text = heroData.rarity.ToString();
        this.attackText.text = heroData.atk.baseValue.ToString();
        this.attackSpeedText.text = heroData.atkSpeed.baseValue.ToString();
        this.attackRangeText.text = heroData.atkRange.baseValue.ToString();
        this.criRateText.text = heroData.criticalRate.baseValue.ToString();
        this.monsterKillCountText.text = "몬스터 처치 수 : " + hero.GetMonsterKillCount().ToString();
    }

    public void HideHeroInfoUI()
    {
        this.hero = null;
        gameObject.SetActive(false);
    }

}

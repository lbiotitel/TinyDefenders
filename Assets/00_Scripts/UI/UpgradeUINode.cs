using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUINode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradeLevelText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private string upgradeSFX;
    public HeroTribe upgradeTribe;
    private int level;
    private int cost;

    private void Awake()
    {
        upgradeButton.onClick.AddListener(Upgrade);
    }

    public void InitUpgradeUI()
    {
        this.level = PlayerStatManager.instance.GetHeroUpgradeLevel(upgradeTribe);
        this.cost = PlayerStatManager.instance.GetHeroUpgradeCost(upgradeTribe);

        upgradeCostText.text = cost.ToString();
        upgradeLevelText.text = "LV" + level.ToString();
    }

    private void Upgrade()
    {
        if (GameManager.instance.IsEnoughGold(cost))
        {
            SoundManager.instance.PlaySfxSound(upgradeSFX,SFXTYPE.UISOUND);
            InGameUI.instance.ClickUpgradeButton(upgradeTribe);
        }
    }
}

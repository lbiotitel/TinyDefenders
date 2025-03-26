using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUINode : MonoBehaviour
{
    public TextMeshProUGUI upgradeLevelText;
    public TextMeshProUGUI upgradeCostText;
    public Button upgradeButton;
    public HeroTribe upgradeTribe;
    private int level;
    private int cost;   

    private void Awake()
    {
        upgradeButton.onClick.AddListener(Upgrade);
    }

    public void InitUpgradeUI()
    {
        this.level = GameManager.instance.GetHeroUpgradeLevel(upgradeTribe);
        this.cost = GameManager.instance.GetHeroUpgradeCost(upgradeTribe);

        upgradeCostText.text = cost.ToString();
        upgradeLevelText.text = "LV" + level.ToString();
    }

    private void Upgrade()
    {
        if (GameManager.instance.IsEnoughGold(cost))
        {
            InGameUI.instance.ClickUpgradeButton(upgradeTribe);
        }
    }
}

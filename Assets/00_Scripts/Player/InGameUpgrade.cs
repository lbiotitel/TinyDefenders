using UnityEngine;

[System.Serializable]
public class HeroUpgradeNode
{
    public HeroTribe heroTribe;
    public int upgradeLevel = 0;
    public int upgradeCost;
}

[System.Serializable]
public class DrawCardUpgradeNode
{
    public int upgradeLevel = 0;
    public int upgradeCost = 10;
}

public class InGameUpgrade : MonoBehaviour
{
    [SerializeField] private HeroUpgradeNode[] heroUpgrades;
    [SerializeField] private int heroUpgradeCostIncrease; 
    [SerializeField] private DrawCardUpgradeNode drawCardUpgrade;
    [SerializeField] private int drawCardUpgradeCostIncrease = 0; 
    [SerializeField] private int drawCardUpgradeProbabilityAmount = 5;


    public void Upgrade(HeroTribe heroTribe)
    {
        for (int i = 0; i < heroUpgrades.Length; i++)
        {
            if (heroUpgrades[i].heroTribe == heroTribe)
            {
                heroUpgrades[i].upgradeLevel++;
                heroUpgrades[i].upgradeCost += heroUpgradeCostIncrease;
                break;
            }
        }
    }

    public void UpgradeCardDrawLevel()
    {
        drawCardUpgrade.upgradeLevel++;
        drawCardUpgrade.upgradeCost += drawCardUpgradeCostIncrease;
        SoDataManager.instance.UpgradeIngameSummonRarity(drawCardUpgradeProbabilityAmount);
        InGameUI.instance.RedrawUpgradeUI();
    }

    public int GetDrawCardUpgradeLevel()
    {
        return drawCardUpgrade.upgradeLevel;
    }

    public int GetDrawCardUpgradeCost()
    {
        return drawCardUpgrade.upgradeCost;
    }

    public void SetHeroUpgradeLevel(HeroTribe heroTribe, int level)
    {
        HeroUpgradeNode heroUpgradeNode = GetHeroUpgradeNode(heroTribe);
        heroUpgradeNode.upgradeLevel = level;
    }

    public void SetHeroUpgradeCost(HeroTribe heroTribe, int cost)
    {
        HeroUpgradeNode heroUpgradeNode = GetHeroUpgradeNode(heroTribe);
        heroUpgradeNode.upgradeCost = cost;
    }

    public int GetHeroUpgradeLevel(HeroTribe heroTribe)
    {
        return GetHeroUpgradeNode(heroTribe).upgradeLevel;
    }

    public int GetHeroUpgradeCost(HeroTribe heroTribe)
    {
        return GetHeroUpgradeNode(heroTribe).upgradeCost;
    }

    private HeroUpgradeNode GetHeroUpgradeNode(HeroTribe heroTribe)
    {
        for (int i = 0; i < heroUpgrades.Length; i++)
        {
            if (heroUpgrades[i].heroTribe == heroTribe)
            {
                return heroUpgrades[i];
            }
        }

        return null;
    }
}

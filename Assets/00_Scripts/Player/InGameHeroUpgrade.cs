using UnityEngine;

[System.Serializable]
public class HeroUpgradeNode {
    public HeroTribe heroTribe;
    public int upgradeLevel = 0;
    public int upgradeCost;
}
public class InGameHeroUpgrade : MonoBehaviour
{
    [SerializeField] private HeroUpgradeNode[] heroUpgarde;
    [SerializeField] private int upgradeIncreaseCost;

    public void Upgrade(HeroTribe heroTribe)
    {
        for (int i = 0; i < heroUpgarde.Length; i++)
        {
            if (heroUpgarde[i].heroTribe == heroTribe)
            {
                heroUpgarde[i].upgradeLevel++;
                heroUpgarde[i].upgradeCost += upgradeIncreaseCost;
                break;
            }
        }
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
        for (int i = 0; i < heroUpgarde.Length; i++)
        {
            if (heroUpgarde[i].heroTribe == heroTribe)
            {
                return heroUpgarde[i];
            }
        }

        return null;
    }
}

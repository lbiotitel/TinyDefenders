using UnityEngine;

[System.Serializable]
public class HeroUpgradeNode {
    public HeroTribe heroTribe;
    public int upgradeLevel = 0;
}
public class InGameHeroUpgrade : MonoBehaviour
{
    [SerializeField] private HeroUpgradeNode[] heroUpgarde;

    public void Upgrade(HeroTribe heroTribe)
    {
        for (int i = 0; i < heroUpgarde.Length; i++)
        {
            if (heroUpgarde[i].heroTribe == heroTribe)
            {
                heroUpgarde[i].upgradeLevel++;
                break;
            }
        }
    }

    public int GetHeroUpgrade(HeroTribe heroTribe)
    {
        for (int i = 0; i < heroUpgarde.Length; i++)
        {
            if (heroUpgarde[i].heroTribe == heroTribe)
            {
                return heroUpgarde[i].upgradeLevel;
            }
        }

        return 0;
    }
}

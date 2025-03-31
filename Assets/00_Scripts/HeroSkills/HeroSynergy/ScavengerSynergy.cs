using UnityEngine;

public class ScavengerSynergy : HeroSynergyBase
{
    [SerializeField] private float[] goldBonus;

    public override void HeroSynergyActive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.AddGoudBonusModifire(playerNetworkType,goldBonus[level - 1]);
        if (level == 3)
        { 
            EventManager.instance.onDrawCardAction += ScavengerDrawGoldCard;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if(level < 1)return;
        PlayerStatManager.instance.RemoveGoldBonusModifire(playerNetworkType, goldBonus[level - 1]);
        if (level == 3)
        {
            EventManager.instance.onDrawCardAction -= ScavengerDrawGoldCard;
        }
    }

    private void ScavengerDrawGoldCard()
    {
        Debug.Log("골드 카드 드로우");
    }
}

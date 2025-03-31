using UnityEngine;

public class DwarfSynergy : HeroSynergyBase
{
    [SerializeField] private float[] heroAtkRange;

    public override void HeroSynergyActive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.AddAtkRangeModifire(playerNetworkType,heroAtkRange[level - 1]);
        if (level == 3)
        {
            EventManager.instance.onHeroSummonAction += DwarfDrawCard;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if(level < 1)return;
        PlayerStatManager.instance.RemoveAtkRangeModifire(playerNetworkType, heroAtkRange[level - 1]);
        if (level == 3)
        { 
            EventManager.instance.onHeroSummonAction -= DwarfDrawCard;
        }
    }

    private void DwarfDrawCard() 
    {
        CardManager.instance.DrawCard();
    }
}

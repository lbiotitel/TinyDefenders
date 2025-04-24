using UnityEngine;

public class DwarfSynergy : HeroSynergyBase
{
    [SerializeField] private float[] heroAtkRange;

    public override void HeroSynergyActive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.AddStatModifire(synergyNetworkType, PlayerStatType.AtkRange, heroAtkRange[level - 1]);
        if (level == 3)
        {
            EventManager.instance.onHeroSpawnAction += DwarfDrawCard;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if(level < 1)return;
        PlayerStatManager.instance.AddStatModifire(synergyNetworkType, PlayerStatType.AtkRange, heroAtkRange[level - 1]);
        if (level == 3)
        { 
            EventManager.instance.onHeroSpawnAction -= DwarfDrawCard;
        }
    }

    private void DwarfDrawCard(Hero hero) 
    {
        CardManager.instance.DrawCard();
    }
}

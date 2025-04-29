using UnityEngine;

public class RiderSynergy : HeroSynergyBase
{
    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
        EventManager.instance.onHeroAttackToMonsterAction += RiderSynergyLogic;
    }

    public override void HeroSynergyDeactive(int level)
    {
        if(!CheckSynergyLevel(level)) return;
        EventManager.instance.onHeroAttackToMonsterAction -= RiderSynergyLogic;
    }


    private void RiderSynergyLogic(Hero hero, Monster monster)
    {
        if (hero.GetHeroData().heroClass != HeroClass.RIDER) return;
        if (hero.heroOwnerNetworkType != synergyNetworkType) return;
    }
}

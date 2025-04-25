using System.Collections.Generic;
using UnityEngine;

public class TankerSynergy : HeroSynergyBase
{
    [SerializeField] private float[] stunProbability;
    [SerializeField] private float[] stunDuration;
    private List<DebuffEffect> debuffEffectList;

    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;

        if (currentSynergyLevel == 1)
        {
            EventManager.instance.onHeroAttackToMonsterAction += TankerSynergyLogic;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (!CheckSynergyLevel(level)) return;

        if (currentSynergyLevel == 1)
        {
            EventManager.instance.onHeroAttackToMonsterAction -= TankerSynergyLogic;
        }
    }


    private void TankerSynergyLogic(Hero hero, Monster monster)
    {
        if (hero.GetHeroData().heroClass != HeroClass.TANKER) return;
        if (hero.heroOwnerNetworkType != synergyNetworkType) return;

        debuffEffectList = new List<DebuffEffect>();

        DebuffEffect debuff = new DebuffEffect();
        debuff.debuffType = DebuffType.STUN;
        debuff.values = new float[] { stunProbability[currentSynergyLevel], stunDuration[currentSynergyLevel] };

        debuffEffectList.Add(debuff);

        NetworkRpcManager.instance.AttackMonster(hero.heroOwnerNetworkType, 0, monster, debuffEffectList);
    }

}

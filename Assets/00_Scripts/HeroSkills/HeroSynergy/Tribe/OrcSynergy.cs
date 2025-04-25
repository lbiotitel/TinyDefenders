using System.Collections.Generic;
using UnityEngine;

public class OrcSynergy : HeroSynergyBase
{
    [SerializeField] private int[] criStackCount;
    private Dictionary<Hero,int> orcStackDic = new Dictionary<Hero,int>();

    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;

        EventManager.instance.onMonsterBeforeTakeDamageAction += OrcSynergyLogic;
        EventManager.instance.onWaveFinishAction += ReleaseAllStack;
    }

    public override void HeroSynergyDeactive(int level)
    {
        if(!CheckSynergyLevel(level)) return;

        ReleaseAllStack();
        EventManager.instance.onMonsterBeforeTakeDamageAction -= OrcSynergyLogic;
        EventManager.instance.onWaveFinishAction -= ReleaseAllStack;
    }

    private void OrcSynergyLogic(DamageEvent damageEvent)
    {
        Hero hero = damageEvent.hero;

        if (hero.GetHeroData().heroTribe != HeroTribe.ORC) return;
        if (hero.heroOwnerNetworkType != synergyNetworkType) return;

        if (!orcStackDic.ContainsKey(hero))
            orcStackDic.Add(hero, 1);
        else
            orcStackDic[hero]++;

        int currentStack = orcStackDic[hero];

        if (currentStack >= criStackCount[currentSynergyLevel])
        {
            if (!damageEvent.isCri)
            {
                Debug.Log("»£√‚");
                damageEvent.isCri = true;
                damageEvent.finalDamage *= hero.GetHeroData().GetHeroCriDamage();
            }

            orcStackDic[hero] = 0;
        }
    }

    private void ReleaseAllStack()
    {
        orcStackDic.Clear();
    }

}

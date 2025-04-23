using System.Collections.Generic;
using UnityEngine;

public class SpearSynergy : HeroSynergyBase
{
    [SerializeField] private float[] atkRangeModifireValue;
    [SerializeField] private float atkSpeedMaxStack;
    [SerializeField] private float atkSpeedStackValue;
    private Dictionary<Hero,int> heroAtkSpeedStackDic = new Dictionary<Hero,int>();

    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
        PlayerStatManager.instance.AddAtkRangeModifire(synergyNetworkType, atkRangeModifireValue[currentSynergyLevel]);
        if (currentSynergyLevel == 2)
        {
            EventManager.instance.onHeroAttackToMonsterAction += SpearSynergyLogic;
            EventManager.instance.onWaveFinishAction += ReleaseAllStack;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
        PlayerStatManager.instance.RemoveAtkRangeModifire(synergyNetworkType, atkRangeModifireValue[currentSynergyLevel]);
        if (currentSynergyLevel == 2)
        {
            ReleaseAllStack();
            EventManager.instance.onHeroAttackToMonsterAction -= SpearSynergyLogic;
            EventManager.instance.onWaveFinishAction -= ReleaseAllStack;
        }
    }

    private void SpearSynergyLogic(Hero hero,Monster monster)
    {
        if (hero.GetHeroData().heroClass != HeroClass.SPEAR) return;
        if (hero.heroOwnerNetworkType != synergyNetworkType) return;
        if (hero == null) return;

        if (!heroAtkSpeedStackDic.ContainsKey(hero)) heroAtkSpeedStackDic.Add(hero,0);

        int currentStack = heroAtkSpeedStackDic[hero];

        if (currentStack < atkSpeedMaxStack)
        {
            hero.GetHeroData().AddStatModifire(StatType.AtkSpeed,atkSpeedStackValue);

            heroAtkSpeedStackDic[hero]++;
        }
    }

    private void ReleaseAllStack()
    {
        foreach (var data in heroAtkSpeedStackDic)
        {
            Hero hero = data.Key;
            if (hero == null) continue;
            int stack = data.Value;

            for (int i = 0; i < stack; i++)
            {
                hero.GetHeroData().RemoveStatModifire(StatType.AtkSpeed,atkSpeedStackValue);
            }
        }

        heroAtkSpeedStackDic.Clear();
    }
}

using System.Collections.Generic;
using UnityEngine;

public class SpearSynergy : HeroSynergyBase
{
    [SerializeField] private float[] atkRangeModifireValue;
    [SerializeField] private float atkSpeedMaxStack;
    [SerializeField] private float atkSpeedStackValue;
    private Dictionary<Hero, int> heroAtkSpeedStackDic = new Dictionary<Hero, int>();

    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
        if (currentSynergyLevel == 3)
        {
            EventManager.instance.onHeroAttackToMonsterAction += SpearSynergyLogic;
            EventManager.instance.onWaveFinishAction += ReleaseAllStack;
        }
        else
        {
            PlayerStatManager.instance.
                AddStatModifire(synergyNetworkType, HeroClass.SPEAR, PlayerStatType.AtkRange, atkRangeModifireValue[currentSynergyLevel]);
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (!CheckSynergyLevel(level)) return;

        if (currentSynergyLevel == 3)
        {
            ReleaseAllStack();
            EventManager.instance.onHeroAttackToMonsterAction -= SpearSynergyLogic;
            EventManager.instance.onWaveFinishAction -= ReleaseAllStack;
        }
        else
        {
            PlayerStatManager.instance.
                RemoveStatModifire(synergyNetworkType, HeroClass.SPEAR, PlayerStatType.AtkRange, atkRangeModifireValue[currentSynergyLevel]);
        }
    }

    private void SpearSynergyLogic(Hero hero, Monster monster)
    {
        if (hero.GetHeroData().heroClass != HeroClass.SPEAR) return;
        if (hero.heroOwnerNetworkType != synergyNetworkType) return;
        if (hero == null) return;

        if (!heroAtkSpeedStackDic.ContainsKey(hero)) heroAtkSpeedStackDic.Add(hero, 0);

        int currentStack = heroAtkSpeedStackDic[hero];

        if (currentStack < atkSpeedMaxStack)
        {
            hero.GetHeroData().AddStatModifire(HeroStatType.AtkSpeed, atkSpeedStackValue);

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
                hero.GetHeroData().RemoveStatModifire(HeroStatType.AtkSpeed, atkSpeedStackValue);
            }
        }

        heroAtkSpeedStackDic.Clear();
    }
}

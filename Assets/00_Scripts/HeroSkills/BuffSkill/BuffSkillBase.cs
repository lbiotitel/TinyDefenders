using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BuffSkillBase : SkillBase
{
    public override bool CheckSkillCanActive()
    {
        return true;
    }

    public override void SkillLogic()
    {
        hero.AnimatorChange("SKILL",true);
    }

    protected List<Hero> FindHeroInRange(float range)
    {
        PlayerNetworkType heroNetworkType = hero.heroOwnerNetworkType;
        List<Hero> heroList = SpawnManager.instance.GetHeroList(heroNetworkType);

        List<Hero> targetHeroList = new List<Hero>();

        foreach (var targetHero in heroList)
        {
            if (Vector2.Distance(targetHero.transform.position, hero.transform.position) <= range)
            { 
                targetHeroList.Add(targetHero);
            }
        }

        return targetHeroList;
    }
}

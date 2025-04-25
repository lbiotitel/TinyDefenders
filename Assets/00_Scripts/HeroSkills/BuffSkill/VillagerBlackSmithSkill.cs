using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerBlackSmithSkill : BuffSkillBase
{
    private List<Hero> buffHeroList = new List<Hero>();

    public override void SkillLogic()
    {
        base.SkillLogic();
        PlayerNetworkType heroNetworkType = hero.heroOwnerNetworkType;


        float buffRange = heroData.skillData.GetBuffRange(heroNetworkType);
        float buffTime = heroData.skillData.GetBuffTime(heroNetworkType);
        float buffAmount = heroData.skillData.GetBuffAmount(heroNetworkType);

        GameObject particle = heroData.skillData.skillPrefab;

        List<Hero> targetHeroList = FindHeroInRange(buffRange);


        foreach (var hero in targetHeroList)
        {
            if (hero.heroOwnerNetworkType != heroNetworkType) continue;
            PoolingManager.instance.GetObject(particle, hero.transform.position);
  
            if (NetUtils.IsHost())
            {
                hero.GetHeroData().AddStatModifire(HeroStatType.CriticalRate, buffAmount);
                buffHeroList.Add(hero);
            }
        }

        if (NetUtils.IsHost())
            StartCoroutine(BuffSkillCoroutine(buffTime,buffAmount));
    }


    private IEnumerator BuffSkillCoroutine(float buffTime,float buffAmount)
    { 
        yield return new WaitForSeconds(buffTime);

        foreach (var hero in buffHeroList)
        {
            hero.GetHeroData().RemoveStatModifire(HeroStatType.CriticalRate, buffAmount);
        }

        buffHeroList.Clear();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerNounSkill : BuffSkillBase
{
    private List<Hero> buffHeroList = new List<Hero>();
    private float buffAmount;
    private float buffRange;
    private float buffTime; 

    public override void SkillLogic()
    {
        base.SkillLogic();

        PlayerNetworkType heroNetworkType = hero.heroOwnerNetworkType;

        buffRange = heroData.skillData.GetBuffRange(heroNetworkType);
        buffTime = heroData.skillData.GetBuffTime(heroNetworkType);
        buffAmount = heroData.skillData.GetBuffAmount(heroNetworkType);

        GameObject particle = heroData.skillData.skillPrefab;

        List<Hero> targetHeroList = FindHeroInRange(buffRange);


        foreach (var hero in targetHeroList)
        {
            if (hero.heroOwnerNetworkType != heroNetworkType) continue;
            PoolingManager.instance.GetObject(particle, hero.transform.position);

            if (NetUtils.IsHost())
            {
                hero.GetHeroData().AddStatModifire(HeroStatType.Atk, buffAmount);
                buffHeroList.Add(hero);
            }
        }

        if (NetUtils.IsHost())
            StartCoroutine(BuffSkillCoroutine());
    }


    private IEnumerator BuffSkillCoroutine()
    {
        yield return new WaitForSeconds(buffTime);
        ClearBuff();
    }


    private void ClearBuff()
    {
        foreach (var hero in buffHeroList)
        {
            hero.GetHeroData().RemoveStatModifire(HeroStatType.Atk, buffAmount);
        }

        buffHeroList.Clear();
    }


    private void OnDestroy()
    {
        if(NetUtils.IsHost())
            ClearBuff();
    }
}

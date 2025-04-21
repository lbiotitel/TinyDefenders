using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CombineCenter : MonoBehaviour
{

    private void Start()
    {
        EventManager.instance.onHeroSummonAction += CheckCanCombineHero;
    }


    private void CheckCanCombineHero(Hero hero)
    {
        if(hero.GetHeroData().heroLevel >= GameManager.instance.maxHeroLevel)return;

        List<Hero> ownHeroList = SpawnManager.instance.GetHeroList(NetUtils.GetPlayerNetworkType());

        List<Hero> requireHeroList = new List<Hero>();
        requireHeroList.Add(hero);

        foreach (var ownHero in ownHeroList)
        {
            if (hero == ownHero) continue;
            if (hero.heroName == ownHero.heroName && 
                hero.heroOwnerNetworkType == ownHero.heroOwnerNetworkType &&
                hero.GetHeroData().heroLevel == ownHero.GetHeroData().heroLevel)
            { 
                requireHeroList.Add(ownHero);
            }
        }

        if (requireHeroList.Count >= GameManager.instance.requireCombineHeroCount)
        {
            CombineHero(requireHeroList);
        }
    }


    private void CombineHero(List<Hero> requireHeroList)
    {
        string name = requireHeroList[0].heroName;
        int heroLevel = requireHeroList[0].GetHeroData().heroLevel + 1;

        foreach (var hero in requireHeroList)
        {
            hero.GetHeroHolder().DespawnHeroServerRpc();
        }

        SpawnManager.instance.Spawn(name, heroLevel);
    }
}

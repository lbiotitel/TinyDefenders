using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CombineCenter : MonoBehaviour
{

    [SerializeField] private int requireCombineCount = 3;

    private void Start()
    {
        EventManager.instance.onHeroSummonAction += CheckCanCombineHero;
    }


    private void CheckCanCombineHero(Hero hero)
    {
        List<Hero> ownHeroList = SpawnManager.instance.GetHeroList(NetUtils.GetPlayerNetworkType());

        List<Hero> requireHeroList = new List<Hero>();
        requireHeroList.Add(hero);

        foreach (var ownHero in ownHeroList)
        {
            if (hero == ownHero) continue;
            if (hero.heroName == ownHero.heroName)
            { 
                requireHeroList.Add(ownHero);
            }
        }

        if (requireHeroList.Count >= requireCombineCount)
        {
            CombineHero(requireHeroList);
        }
    }


    private void CombineHero(List<Hero> requireHeroList)
    {
        string name = requireHeroList[0].heroName;

        foreach (var hero in requireHeroList)
        {
            hero.GetHeroHolder().DespawnHeroServerRpc();
        }

        SpawnManager.instance.Summon(name);
    }
}

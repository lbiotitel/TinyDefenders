using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArcherSynergy : HeroSynergyBase
{
    [SerializeField] private int[] multiTargetValue;
    [SerializeField] private float[] multiTargetProbability;
    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
        if (currentSynergyLevel == 1 || currentSynergyLevel == 2 || currentSynergyLevel == 3)
        {
            EventManager.instance.onBeforeAttackMonsterAction += ArcherSynergyLogic;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
        if (currentSynergyLevel == 1 || currentSynergyLevel == 2 || currentSynergyLevel == 3)
        {
            EventManager.instance.onBeforeAttackMonsterAction -= ArcherSynergyLogic;
        }
    }


    private void ArcherSynergyLogic(Hero hero,Monster monster)
    {
        if (hero.GetHeroData().heroClass != HeroClass.ARCHER) return;
        if (hero.heroOwnerNetworkType != synergyNetworkType) return;

        MathUtils.RandomChanceAction(multiTargetProbability[currentSynergyLevel], () => {

            List<Monster> monsterList = hero.GetMonsterList();
            List<Monster> targetMonsters = new List<Monster>();

            foreach (var m in monsterList)
            {
                if (targetMonsters.Count == multiTargetValue[currentSynergyLevel]) break;
                if (m == monster) continue;
                if (m.IsDead()) continue;
                targetMonsters.Add(m);
            }

            HeroScriptable heroData = hero.GetHeroData();
            BaseProjectile projectileRrefab = heroData.projectile;

            foreach (var m in targetMonsters)
            {
                GameObject go = PoolingManager.instance.GetObject(projectileRrefab.gameObject, hero.transform.position);

                BaseProjectile projectile = go.GetComponent<BaseProjectile>();
                projectile.Init(m.transform, hero, heroData.GetHeroAtk(), heroData.debuffEffects);
            }
        });
    }
}

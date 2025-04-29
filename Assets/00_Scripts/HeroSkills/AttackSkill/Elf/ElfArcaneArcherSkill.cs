using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ElfArcaneArcherSkill : AttackSkillBase
{
    public override void SkillLogic()
    {
        base.SkillLogic();
        List<Monster> monsterList = FindCloseMonster(heroData.skillData.targetCount);

        Monster targetMonster = null;
        foreach (var monster in monsterList)
        {
            if (monster.IsDead()) continue;
            targetMonster = monster;
            break;
        }

        GameObject go = PoolingManager.instance.GetObject(heroData.skillData.skillPrefab, targetMonster.transform.position);
        go.GetComponent<BaseProjectile>().Init(targetMonster, hero, heroData.GetHeroSkillDamage());

    }
}

using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ElfArcaneArcherSkill : AttackSkillBase
{
    public override void SkillLogic()
    {
        base.SkillLogic();
        List<Monster> monsterList = FindCloseMonster(heroData.skillData.targetCount);

        Transform monsterTransform = null;
        foreach (var monster in monsterList)
        {
            if (monster.IsDead()) continue;
            monsterTransform = monster.transform;
            break;
        }

        GameObject go = PoolingManager.instance.GetObject(heroData.skillData.skillPrefab, monsterTransform.position);
        go.GetComponent<BaseProjectile>().Init(monsterTransform, hero, heroData.GetHeroSkillDamage());

    }
}

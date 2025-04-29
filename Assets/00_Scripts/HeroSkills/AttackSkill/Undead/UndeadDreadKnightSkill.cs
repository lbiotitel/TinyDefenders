using System.Collections.Generic;
using UnityEngine;

public class UndeadDreadKnightSkill : AttackSkillBase
{

    public override void SkillLogic()
    {
        base.SkillLogic();
        List<Monster> monsterList = FindCloseMonster(heroData.skillData.targetCount);

        foreach (var monster in monsterList)
        {
            if (monster.IsDead()) continue;
            GameObject go = PoolingManager.instance.GetObject(heroData.skillData.skillPrefab, monster.transform.position);
            go.GetComponent<BaseProjectile>().Init(monster, hero, heroData.GetHeroSkillDamage());
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class HumanKingSkill : AttackSkillBase
{

    public override void SkillLogic()
    {
        base.SkillLogic();

        int targetCount = skillData.targetCount;

        List<Monster> monsterList = FindCloseMonster(targetCount);

        foreach (var monster in monsterList)
        {
            if (monster.IsDead()) continue;
            GameObject go = PoolingManager.instance.GetObject(skillData.skillPrefab,transform.position);
            go.GetComponent<BaseProjectile>().Init(monster.transform,hero,heroData.GetHeroSkillDamage());
        }
    }
}

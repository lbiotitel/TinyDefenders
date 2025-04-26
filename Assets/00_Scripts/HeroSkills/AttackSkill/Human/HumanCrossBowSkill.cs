using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HumanCrossBowSkill : AttackSkillBase
{

    public override void SkillLogic()
    {
        base.SkillLogic();

        int targetCount = skillData.targetCount;

        List<Monster> monsterList = FindCloseMonster(targetCount);

        if (monsterList.Count > 0)
        {
            Monster monster = monsterList[0];

            GameObject go = PoolingManager.instance.GetObject(skillData.skillPrefab, transform.position);
            go.GetComponent<BaseProjectile>().Init(monster.transform, hero, heroData.GetHeroSkillDamage());
        }
    }
}

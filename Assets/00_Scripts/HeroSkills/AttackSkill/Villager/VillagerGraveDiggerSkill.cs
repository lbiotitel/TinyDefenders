using System.Collections.Generic;
using UnityEngine;

public class VillagerGraveDiggerSkill : AttackSkillBase
{


    public override void SkillLogic()
    {
        base.SkillLogic();

        SkillData skillData = hero.GetHeroData().skillData;
        int targetCount = skillData.targetCount;

        List<Monster> monsterList = FindCloseMonster(targetCount);

        foreach (var monster in monsterList)
        {
            if (monster.IsDead()) continue;
            GameObject go = PoolingManager.instance.GetObject(skillData.skillPrefab, monster.transform.position + new Vector3(0,5,0));

            DamageData damageData = GetBasicSkillDamageData(monster);
            go.GetComponent<Projectile>().Init(damageData);

        }
    }
}

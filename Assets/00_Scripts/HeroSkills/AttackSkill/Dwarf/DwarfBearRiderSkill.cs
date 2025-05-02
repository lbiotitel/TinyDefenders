using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DwarfBearRiderSkill : AttackSkillBase
{

    public override void SkillLogic()
    {
        base.SkillLogic();

        int targetCount = skillData.targetCount;

        List<Monster> monsterList = FindCloseMonster(targetCount);

        if (monsterList.Count > 0)
        { 
            Monster monster = monsterList[0];
            Projectile projectile = 
                PoolingManager.instance.GetObject<Projectile>(skillData.skillPrefab,transform.position);

            DamageData damageData = GetBasicSkillDamageData(monster);

            projectile.Init(damageData);
        }
    }

}

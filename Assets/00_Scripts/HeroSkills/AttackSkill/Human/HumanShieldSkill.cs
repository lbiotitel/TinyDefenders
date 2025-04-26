using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HumanShieldSkill : AttackSkillBase
{

    public override void SkillLogic()
    {
        base.SkillLogic();

        int targetCount = skillData.targetCount;

        List<Monster> monsterList = FindCloseMonster(targetCount);

        PoolingManager.instance.GetObject(skillData.skillPrefab,transform.position);

        if (monsterList.Count > 0)
        {
            Monster monster = monsterList[0];
            NetworkRpcManager.instance.AttackMonster(hero.heroOwnerNetworkType,heroData.GetHeroSkillDamage(),monster,heroData.skillDebuffEffects);
        }
    }
}

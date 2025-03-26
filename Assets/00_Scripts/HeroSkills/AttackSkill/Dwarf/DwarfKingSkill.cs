using System.Collections.Generic;
using UnityEngine;

public class DwarfKingSkill : AttackSkillBase
{

    public override void SkillLogic()
    {
        base.SkillLogic();
        List<Monster> monsterList = hero.GetMonsterList();

        int count = 0;
        foreach (var monster in monsterList)
        {
            if (monster.IsDead()) continue;
            if (count == heroData.skillData.targetCount) break;
            GameObject go = PoolingManager.instance.GetObject(heroData.skillData.skillPrefab, monster.transform.position);
            go.GetComponent<BaseProjectile>().Init(monster.transform, hero, hero.GetHeroSkillDamage());
            count++;
        }
    }
}

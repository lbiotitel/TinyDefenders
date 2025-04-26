using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanFireMageSkill : AttackSkillBase
{

    public override void SkillLogic()
    {
        base.SkillLogic();

        int targetCount = skillData.targetCount;

        List<Monster> monsterList = FindCloseMonster(targetCount);

        StartCoroutine(FireMageSkillCoroutine(monsterList));
    }

    private IEnumerator FireMageSkillCoroutine(List<Monster> monsterList)
    {

        foreach (var monster in monsterList)
        {
            GameObject go = PoolingManager.instance.GetObject(skillData.skillPrefab, transform.position);
            go.GetComponent<BaseProjectile>().Init(monster.transform, hero, heroData.GetHeroSkillDamage());

            yield return new WaitForSeconds(0.1f);
        }
    }
}

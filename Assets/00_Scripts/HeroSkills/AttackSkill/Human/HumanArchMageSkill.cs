using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanArchMageSkill : AttackSkillBase
{

    public override void SkillLogic()
    {
        base.SkillLogic();

        int targetCount = skillData.targetCount;

        List<Monster> monsterList = FindCloseMonster(targetCount);

        StartCoroutine(HumanArchMageCouroutine(monsterList));
    }

    private IEnumerator HumanArchMageCouroutine(List<Monster> monsterList)
    {
        foreach (var monster in monsterList)
        {
            GameObject go = PoolingManager.instance.GetObject(skillData.skillPrefab, transform.position + (Random.insideUnitSphere * 1.5f));
            go.GetComponent<BaseProjectile>().Init(monster.transform, hero, heroData.GetHeroSkillDamage());

            yield return new WaitForSeconds(0.05f);
        }
    }
}

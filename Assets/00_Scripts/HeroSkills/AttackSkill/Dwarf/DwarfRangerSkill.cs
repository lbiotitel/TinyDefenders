using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwarfRangerSkill : AttackSkillBase
{


    public override void SkillLogic()
    {
        base.SkillLogic();
        List<Monster> monsterList = FindCloseMonster(1);
        StartCoroutine(SkillLogicCoroutine(monsterList[0]));
    }

    private IEnumerator SkillLogicCoroutine(Monster monster)
    {

        for (int i = 0; i < heroData.skillData.targetCount; i++)
        {
            GameObject go = PoolingManager.instance.GetObject(heroData.skillData.skillPrefab, transform.position);
            go.GetComponent<BaseProjectile>().Init(monster, hero, heroData.GetHeroSkillDamage());
            yield return new WaitForSeconds(0.1f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengerCaptainSkill : AttackSkillBase
{


    public override void SkillLogic()
    {
        StartCoroutine(SkillLogicCoroutine());
    }

    private IEnumerator SkillLogicCoroutine()
    {
        for (int i = 0; i < heroData.skillData.skillDuration; i++)
        {
            List<Monster> monsterList = FindCloseMonster(heroData.skillData.targetCount);

            foreach (var monster in monsterList)
            {
                GameObject go = PoolingManager.instance.GetObject(heroData.skillData.skillPrefab, monster.transform.position
                    + new Vector3(0, 3, 0));

                DamageData damageData = GetBasicSkillDamageData();
                damageData.Target = monster;

                go.GetComponent<Projectile>().Init(damageData);
            }

            yield return new WaitForSeconds(1f);
        }
    }
}

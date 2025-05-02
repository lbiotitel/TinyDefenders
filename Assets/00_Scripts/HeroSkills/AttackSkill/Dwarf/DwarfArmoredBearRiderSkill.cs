using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DwarfArmoredBearRiderSkill : AttackSkillBase
{
    private Coroutine _coSkill;
    public override void SkillLogic()
    {
        base.SkillLogic();
        
        GameObject skillObject = PoolingManager.instance.GetObject(skillData.skillPrefab, transform.position);
        skillObject.transform.SetParent(transform);

        StartCoroutine(SkillCouroutine(skillObject));
    }

    private IEnumerator SkillCouroutine(GameObject skillObject)
    {
        float duration = skillData.skillDuration;
        float elapsedTime = 0f;
        float damageInterval = skillData.skillDamageInterval;
        float damageElapsedTime = 0f;

        while (elapsedTime < duration)
        { 
            elapsedTime += Time.deltaTime;
            damageElapsedTime += Time.deltaTime;

            skillObject.transform.Rotate(0,0,10f);

            if (damageElapsedTime > damageInterval)
            {
                damageElapsedTime = 0;

                List<Monster> monsterList = FindCloseMonster(skillData.targetCount);

                foreach (var monster in monsterList)
                {
                    if (monster.IsDead()) continue;
                    NetworkRpcManager.instance.AttackMonster(
                        hero.heroOwnerNetworkType,
                        heroData.GetHeroSkillDamage(),
                        monster,
                        heroData.skillDebuffEffects
                        );
                }
            }

            yield return null;
        }

        PoolingManager.instance.ReturnObject(skillObject);
    }

    private void OnDestroy()
    {
        if (_coSkill != null)
        {
            StopCoroutine(_coSkill);
            _coSkill = null;
        }
    }
}

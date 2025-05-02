using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfWardenSkill : AttackSkillBase
{

    private Coroutine _coSkill;

    public override void SkillLogic()
    {
        base.SkillLogic();
        List<Monster> monsterList = FindCloseMonster(1);
        _coSkill = StartCoroutine(SkillLogicCoroutine(monsterList[0]));
    }

    private IEnumerator SkillLogicCoroutine(Monster monster)
    {

        for (int i = 0; i < heroData.skillData.targetCount; i++)
        {
            if(monster.IsDead()) break;

            GameObject go = PoolingManager.instance.GetObject(heroData.skillData.skillPrefab, transform.position);

            DamageData damageData = GetBasicSkillDamageData(monster);

            go.GetComponent<Projectile>().Init(damageData);
            yield return new WaitForSeconds(0.1f);
        }
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

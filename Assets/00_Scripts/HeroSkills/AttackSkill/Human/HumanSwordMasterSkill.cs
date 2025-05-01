using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class HumanSwordMasterSkill : AttackSkillBase
{
    private bool isSkillOn = false;
    ParticleSystem particle;

    public override void SkillLogic()
    {
        base.SkillLogic();

        isSkillOn = true;

        GameObject go = PoolingManager.instance.GetObject(skillData.particlePrefab,transform.position);

        particle = go.GetComponent<ParticleSystem>();
        var mainModule = particle.main;
        mainModule.duration = skillData.skillDuration;
        particle.Play();

        EventManager.instance.onBeforeAttackMonsterAction += SwordMasterSkillLogic;

        StartCoroutine(SwordMasterSkillCouroutine(skillData.skillDuration));
    }

    private void SwordMasterSkillLogic(Hero hero, Monster mainMonster)
    {
        if (this.hero != hero) return;

        int targetCount = skillData.targetCount;
        List<Monster> monsterList = FindCloseMonster(targetCount);

        foreach (var monster in monsterList)
        {
            GameObject go = PoolingManager.instance.GetObject(skillData.skillPrefab, transform.position);

            DamageData damageData = GetBasicSkillDamageData();
            damageData.Target = monster;

            go.GetComponent<Projectile>().Init(damageData);
        }
    }


    private IEnumerator SwordMasterSkillCouroutine(float skillTime) 
    {
        yield return new WaitForSeconds(skillTime);
        isSkillOn = false;
        particle = null;
        EventManager.instance.onBeforeAttackMonsterAction -= SwordMasterSkillLogic;
    }

    private void OnDestroy()
    {
        if (isSkillOn)
        {
            EventManager.instance.onBeforeAttackMonsterAction -= SwordMasterSkillLogic;

            if (particle != null)
                PoolingManager.instance.ReturnObject(particle.gameObject);
        }
    }
}

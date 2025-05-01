using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwarArmoredBearRiderSkillProjectile : Projectile
{
    [SerializeField] private float damageCount;
    [SerializeField] private float atkRange;
/*
    public override void Init(Monster target, Hero referenceHero,double damage, List<DebuffEffect> debuffEffects = null)
    {
        base.Init(target, referenceHero,damage, debuffEffects);
        StartCoroutine(AttackMonsterCoroutine());
        DisableObject(lifeTime);
    }

    protected override void ProjectileMoveLogic()
    {
        transform.Rotate(0, 0, speed);
        transform.position = referenceHero.transform.position;
    }

    protected override void Update()
    {
        ProjectileMoveLogic();
    }

    protected override void HitMonster()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,
            atkRange, LayerMask.GetMask(LayerName.Monster));

        foreach (var collider in colliders)
        {
            if (collider.GetComponent<Monster>().IsDead()) continue;
            AttackMonster(collider.gameObject);
        }
    }

    private IEnumerator AttackMonsterCoroutine()
    {
        float attackInterval = lifeTime / damageCount;

        for (int i = 0; i < damageCount; i++)
        {
            HitMonster();
            yield return new WaitForSeconds(attackInterval);
        }

    }*/
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectile : BaseProjectile
{
    private LineRenderer lineRenderer;

    public override void Init(Transform target, Hero referenceHero, double damage, List<DebuffEffect> debuffEffects = null)
    {
        base.Init(target, referenceHero, damage, debuffEffects);
        lineRenderer = GetComponent<LineRenderer>();
    }

    protected override void ProjectileLogic()
    {
        lineRenderer.SetPosition(0, referenceHero.transform.position);
        lineRenderer.SetPosition(1, target.transform.position);
        AttackMonster(target.gameObject);
        DisableObject(lifeTime);
    }

  
}

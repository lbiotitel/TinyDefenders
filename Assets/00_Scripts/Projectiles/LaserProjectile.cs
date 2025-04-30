using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectile : BaseProjectile
{
    private LineRenderer lineRenderer;

    public override void Init(Monster target, Hero referenceHero, double damage, List<DebuffEffect> debuffEffects = null)
    {
        lineRenderer = GetComponent<LineRenderer>();
        base.Init(target, referenceHero, damage, debuffEffects);
    }

    protected override void ProjectileMoveLogic()
    {
        lineRenderer.SetPosition(0, referenceHero.transform.position);
        lineRenderer.SetPosition(1, target.transform.position);
        HitMonster();
        DisableObject(lifeTime);
    }

  
}

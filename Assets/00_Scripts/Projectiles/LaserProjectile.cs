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
        StartCoroutine(HitMonsterCoroutine(lifeTime));
    }

    protected override void HitMonster()
    {
        destroyParticle = PoolingManager.instance.GetObject(destroyParticle, transform.position);

        if (NetUtils.IsHost() && target != null && !target.GetComponent<Monster>().IsDead())
        {
            NetworkRpcManager.instance.AttackMonster(NetUtils.IsHost(), damage, referenceHero, target.GetComponent<Monster>(),
                referenceHero.GetHeroData().debuffEffects);
        }

        Destroy(this.gameObject);
    }

    private IEnumerator HitMonsterCoroutine(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        HitMonster();
    }

}

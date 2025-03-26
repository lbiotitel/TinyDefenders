using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class LaserProjectile : BaseProjectile
{
    private LineRenderer lineRenderer;

    public override void Init(Transform target, Hero referenceHero, float skillDamage = 1f)
    {
        base.Init(target, referenceHero,skillDamage);
        lineRenderer = GetComponent<LineRenderer>();
    }

    protected override void ChasingTarget()
    {
        lineRenderer.SetPosition(0,referenceHero.transform.position);
        lineRenderer.SetPosition(1, target.transform.position);
        StartCoroutine(HitMonsterCoroutine(lifeTime));
    }

    protected override void HitMonster()
    {
        destroyParticle = PoolingManager.instance.GetObject(destroyParticle, transform.position);

        if (NetUtils.IsServer() && target != null && !target.GetComponent<Monster>().IsDead())
        {
            NetworkRpcManager.instance.AttackMonster(NetUtils.IsServer(), referenceHero.GetHeroAtk(), target.GetComponent<Monster>(),
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

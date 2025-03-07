using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class LaserProjectile : BaseProjectile
{
    private LineRenderer lineRenderer;

    public override void Init(Transform target, Hero referenceHero)
    {
        base.Init(target, referenceHero);
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
        Instantiate(destroyParticle, target.transform.position, Quaternion.identity);

        if (NetUtils.IsServer() && target != null && !target.GetComponent<Monster>().IsDead())
            referenceHero.AttackMonster(target.GetComponent<Monster>());

        Destroy(this.gameObject);
    }

    private IEnumerator HitMonsterCoroutine(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);  
        HitMonster();
    }

}

using System.Collections;
using UnityEngine;

public class LaserProjectile : BaseProjectile
{
    [Header("Laser Value")]
    [SerializeField] private float lifeTime = 0.3f;
    private TrailRenderer trailRenderer;

    public override void Init(Transform target, Hero referenceHero)
    {
        base.Init(target, referenceHero);
        trailRenderer = GetComponent<TrailRenderer>();
    }

    protected override void ChasingTarget()
    {
        trailRenderer.Clear();
        trailRenderer.AddPosition(referenceHero.transform.position);
        trailRenderer.AddPosition(target.transform.position);
        StartCoroutine(HitMonsterCoroutine(lifeTime));
    }

    private IEnumerator HitMonsterCoroutine(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);  
        HitMonster();
    }

}

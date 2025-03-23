using UnityEngine;

public class MeleeProjectile : BaseProjectile
{
    [SerializeField] private bool currentTransformParticle = false;

    public override void Init(Transform target, Hero referenceHero)
    {
        base.Init(target, referenceHero);
        transform.position = currentTransformParticle ?  transform.position:
            target.transform.position;
        transform.Rotate(0, 90, 0);
        HitMonster();
    }

    protected override void HitMonster()
    {
        if (!isAttack)
        {
            isAttack = true;

            destroyParticle = PoolingManager.instance.GetObject(destroyParticlePrefab, transform.position);

            AttackMonster(target.gameObject);

            DisableObject(lifeTime);
        }
    }


    protected override void ChasingTarget() { }

}

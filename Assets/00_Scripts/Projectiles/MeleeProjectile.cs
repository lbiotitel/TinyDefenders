using UnityEngine;

public class MeleeProjectile : BaseProjectile
{
    [SerializeField] private bool currentTransformParticle = false;

    public override void Init(Transform target, Hero referenceHero)
    {
        base.Init(target, referenceHero);
        transform.position = currentTransformParticle ?  transform.position:
            target.transform.position;
        HitMonster();
    }

    protected override void HitMonster()
    {
        if (!isAttack)
        {
            isAttack = true;

            destroyParticle = PoolingManager.instance.GetObject("MonsterHitParticle", transform.position);

            if (NetUtils.IsServer() && target != null && !target.GetComponent<Monster>().IsDead())
                referenceHero.AttackMonster(target.GetComponent<Monster>());

        }
    }

    protected override void ChasingTarget() { }

}

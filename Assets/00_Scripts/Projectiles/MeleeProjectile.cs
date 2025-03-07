using UnityEngine;

public class MeleeProjectile : BaseProjectile
{
    public override void Init(Transform target, Hero referenceHero)
    {
        base.Init(target, referenceHero);
        transform.position = target.transform.position;
        HitMonster();
    }

    protected override void HitMonster()
    {
        destroyParticle = PoolingManager.instance.GetObject("MonsterHitParticle", transform.position);

        if (NetUtils.IsServer() && target != null && !target.GetComponent<Monster>().IsDead())
            referenceHero.AttackMonster(target.GetComponent<Monster>());

    }

    protected override void ChasingTarget() { }

}

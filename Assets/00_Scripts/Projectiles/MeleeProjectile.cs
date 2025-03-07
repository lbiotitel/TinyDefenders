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
        Instantiate(destroyParticle, transform.position, Quaternion.identity);

        if (NetUtils.IsServer() && target != null && !target.GetComponent<Monster>().IsDead())
            referenceHero.AttackMonster(target.GetComponent<Monster>());

    }

    protected override void ChasingTarget() { }

}

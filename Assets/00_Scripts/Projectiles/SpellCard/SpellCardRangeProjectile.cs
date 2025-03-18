using UnityEngine;

public class SpellCardRangeProjectile : SpellCardBaseProjectile
{


    protected override void HitMonster()
    {
        if (!isAttack)
        {
            isAttack = true;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,referenceCard.atkRange, LayerMask.GetMask(LayerName.Monster));
            destroyParticle = PoolingManager.instance.GetObject("MonsterHitParticle", transform.position);

            foreach (var collider in colliders)
            {
                if (NetUtils.IsServer() && collider != null && !collider.GetComponent<Monster>().IsDead())
                {
                    NetworkRpcManager.instance.AttackMonster(NetUtils.IsServer(), referenceCard.damage, collider.GetComponent<Monster>());
                }
            }

            Destroy(this.gameObject, lifeTime);
        }
    }
}

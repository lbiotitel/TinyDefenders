using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WideAreaProjectile : BaseProjectile
{
    [Header("Wide Area Value")]
    [SerializeField] private float range;
    [SerializeField] private int targetCount;


    protected override void HitMonster()
    {
        if (!isAttack)
        {
            isAttack = true;

            int currentCount = 0;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,
                range, LayerMask.GetMask(LayerName.Monster));

            List<Monster> monsters = new List<Monster>();

            foreach (Collider2D collider in colliders)
            {
                if (currentCount >= targetCount) break;
                if (collider.GetComponent<Monster>().IsDead()) continue;

                monsters.Add(collider.GetComponent<Monster>());
                currentCount++;
            }

            foreach (var monster in monsters)
            {
                if (NetUtils.IsServer() && monster != null && !monster.GetComponent<Monster>().IsDead())
                {
                    NetworkRpcManager.instance.AttackMonster(NetUtils.IsServer(), referenceHero.atk, monster.GetComponent<Monster>(),
                        referenceHero.GetHeroData().debuffEffects);
                }
            }

            destroyParticle = PoolingManager.instance.GetObject(destroyParticlePrefab, transform.position);


            DisableObject(lifeTime);
        }
    }
}

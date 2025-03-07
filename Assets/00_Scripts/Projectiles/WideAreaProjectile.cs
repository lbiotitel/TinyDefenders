using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WideAreaProjectile : BaseProjectile
{
    [Header("Wide Area Value")]
    [SerializeField] private float range;
    [SerializeField] private int targetCount;

    private void RangeDamage()
    {
        int currentCount = 0;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,
            range,LayerMask.GetMask(LayerName.Monster));

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
            referenceHero.AttackMonster(monster);
        }
    }



    protected override void HitMonster()
    {
        Instantiate(destroyParticle, transform.position, Quaternion.identity);

        if (NetUtils.IsServer() && target != null)
        {
            referenceHero.AttackMonster(target.GetComponent<Monster>());
            RangeDamage();
        }

        Destroy(this.gameObject);
    }
}

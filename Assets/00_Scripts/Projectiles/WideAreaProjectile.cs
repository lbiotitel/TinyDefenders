using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WideAreaProjectile : BaseProjectile
{
    [Header("Wide Area Value")]
    [SerializeField] private float _range;
    [SerializeField] private int _targetCount;
    public int TargetCount { get { return _targetCount; } set { _targetCount = value; } }


    protected override void HitMonster()
    {
        if (!isAttack)
        {
            isAttack = true;

            int currentCount = 0;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,
                _range, LayerMask.GetMask(LayerName.Monster));

            List<Monster> monsters = new List<Monster>();

            foreach (Collider2D collider in colliders)
            {
                if (currentCount >= _targetCount) break;
                if (collider.GetComponent<Monster>().IsDead()) continue;

                monsters.Add(collider.GetComponent<Monster>());
                currentCount++;
            }

            foreach (var monster in monsters)
            {
                AttackMonster(monster.gameObject);
            }

            destroyParticle = PoolingManager.instance.GetObject(destroyParticlePrefab, transform.position);


            DisableObject(lifeTime);
        }
    }
}

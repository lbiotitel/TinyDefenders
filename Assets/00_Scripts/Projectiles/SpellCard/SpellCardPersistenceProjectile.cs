using System.Collections;
using UnityEngine;

public class SpellCardPersistenceProjectile : SpellCardBaseProjectile
{
    [Header("Persistence Projectile Property")]
    [SerializeField] private int tickCount;
    protected override void HitMonster()
    {
        if (!isAttack)
        {
            isAttack = true;

            StartCoroutine(HitMonsterCoroutine());
        }
    }

    private IEnumerator HitMonsterCoroutine()
    {
        float tickTime = lifeTime / tickCount;
        double tickDamage = referenceCard.damage / tickCount;

        for (int i = 0; i < tickCount; i++)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, referenceCard.atkRange, LayerMask.GetMask(LayerName.Monster));

            foreach (var collider in colliders)
            {
                AttackMonster(collider.gameObject,tickDamage);
            }

            yield return new WaitForSeconds(tickTime);
        }

        Destroy(gameObject);
    }

}

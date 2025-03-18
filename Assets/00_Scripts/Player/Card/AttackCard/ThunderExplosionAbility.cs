using UnityEngine;

public class ThunderExplosionAbility : CardAbilityBase
{
    [SerializeField] private SpellCardBaseProjectile thunderExplosionPrefab;
    [SerializeField] private LayerMask monsterLayer;

    public override void ActiveAbility()
    {
        SpellCardBaseProjectile projectile = Instantiate(thunderExplosionPrefab, transform.position, Quaternion.identity);
        projectile.Init(transform, cardSO as AttackCardSO);
    }
}

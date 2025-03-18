using UnityEngine;

public class TornadoAbility : CardAbilityBase
{
    [SerializeField] private SpellCardBaseProjectile tornadoPrefab;
    [SerializeField] private LayerMask monsterLayer;


    public override void ActiveAbility()
    {
        SpellCardBaseProjectile projectile = Instantiate(tornadoPrefab, transform.position, Quaternion.identity);
        projectile.Init(transform, cardSO as AttackCardSO);
    }
}

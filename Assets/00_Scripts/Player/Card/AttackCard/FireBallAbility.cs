using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class FireBallAbility : CardAbilityBase
{
    [SerializeField] private SpellCardBaseProjectile fireBallPrefab;
    [SerializeField] private LayerMask monsterLayer;


    public override void ActiveAbility()
    {
        SpellCardBaseProjectile projectile = Instantiate(fireBallPrefab, transform.position + new Vector3(0, 5), Quaternion.identity);
        projectile.Init(transform, cardSO as AttackCardSO);
    }

}

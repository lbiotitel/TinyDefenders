using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class FireBallAbility : CardAbilityBase
{
    [SerializeField] private SpellCardBaseProjectile fireBallPrefab;
    [SerializeField] private LayerMask monsterLayer;


    public override void ActiveAbility()
    {
        SpellCardBaseProjectile projectile = PoolingManager.instance.GetObject<SpellCardBaseProjectile>(fireBallPrefab.gameObject, transform.position + new Vector3(0, 5));
        projectile.Init(transform, cardSO as AttackCardSO);
    }

}

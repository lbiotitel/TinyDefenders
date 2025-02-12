using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Hero : Entity
{
    [field:SerializeField] public float attackRange { get; set; } = 1f;
    [field: SerializeField] public float attackSpeed { get; private set; } = 1.0f;
    [field: SerializeField] public LayerMask monsterLayer { get; private set; }
    public NetworkObject target { get; set; } = null;

    public override void Start()
    {
        base.Start();
    }


    private void Update()
    {
        CheckForEnemies();
    }

    private void CheckForEnemies()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position,attackRange, monsterLayer);
        attackSpeed += Time.deltaTime;

        if (enemiesInRange.Length > 0)
        {
            target = enemiesInRange[0].transform.GetComponent<NetworkObject>();
            if (attackSpeed >= 1.0f)
            {
                attackSpeed = 0.0f;
                AnimatorChange("ATTACK", true);
                AttackMonsterServerRpc(target.NetworkObjectId);
            }
        }
        else
            target = null;
    }

    [ServerRpc(RequireOwnership = false)]
    private void AttackMonsterServerRpc(ulong monsterId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(monsterId,out NetworkObject networkObject))
        {
            Monster monster = networkObject.GetComponent<Monster>();
            if (monster != null)
            {
                if (monster.IsDead()) return;
                monster.GetDamage(10);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

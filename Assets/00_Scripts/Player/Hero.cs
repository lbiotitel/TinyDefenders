using System.Collections;
using UnityEngine;

public class Hero : Entity
{
    [field:SerializeField] public float attackRange { get; set; } = 1f;
    [field: SerializeField] public float attackSpeed { get; private set; } = 1.0f;
    [field: SerializeField] public LayerMask monsterLayer { get; private set; }
    public Monster target { get; set; } = null;

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
            target = enemiesInRange[0].transform.GetComponent<Monster>();
            if (attackSpeed >= 1.0f)
            {
                attackSpeed = 0.0f;
                AttackEnemy(target);
            }
        }
        else
            target = null;
    }


    private void AttackEnemy(Monster enemy)
    {
        if (enemy == null) return;
        AnimatorChange("ATTACK",true);
        enemy.GetDamage(10);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

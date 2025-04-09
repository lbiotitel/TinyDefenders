using UnityEngine;

public class RelicDynamite : RelicBase
{
    [SerializeField] private float explosionChance;
    [SerializeField] private float explosionRange;
    [SerializeField] private float explosionDamage;
    [SerializeField] private GameObject explosionParticlePrefab;

    public override void ActiveRelic()
    {
        EventManager.instance.onHeroAttackToMonsterAction += Explosion;
    }

    private void Explosion(Hero hero, Monster monster)
    {
        if (monster.IsDead()) return;

        double atk = hero.GetHeroData().GetHeroAtk();
        double damage = atk * (explosionDamage / 100f);

        NetworkRpcManager.instance.AttackMonster(playerNetworkType, damage, hero, monster);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(monster.transform.position, explosionRange);

        foreach (var collider in colliders)
        {
            Monster mo = collider.GetComponent<Monster>();
            if (mo.IsDead()) continue;

            NetworkRpcManager.instance.AttackMonster(playerNetworkType,damage,hero,mo);
        }

        PoolingManager.instance.GetObject(explosionParticlePrefab, monster.transform.position);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneArcherSkillProjectile : MeleeProjectile
{
    [SerializeField]private float damageCount;

    List<Monster> monsterList = new List<Monster>();
    public override void Init(Monster target, Hero referenceHero, double damage, List<DebuffEffect> debuffEffects = null)
    {
        double tickDamage = damage / damageCount;
        base.Init(target, referenceHero, tickDamage, debuffEffects);
        StartCoroutine(DamageCoroutine());
    }


    private IEnumerator DamageCoroutine()
    {
        float t = lifeTime / damageCount;

        for (int i = 0; i < damageCount; i++)
        {
            foreach (var monster in monsterList)
            {
                if (monster == null) continue;
                if (monster.IsDead()) continue;
                AttackMonster(monster.gameObject);
            }
            yield return new WaitForSeconds(t);
        }

        DisableObject(0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Monster")
        { 
            Monster monster = collision.GetComponent<Monster>();
            if (monster.IsDead()) return;
            monsterList.Add(monster);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Monster")
        {
            Monster monster = collision.GetComponent<Monster>();
            monsterList.Remove(monster);
        }
    }
}

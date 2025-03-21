using UnityEngine;

public class FireMageCatSkill : AttackSkillBase
{


    protected override void ActiveSkill()
    {
        HeroHolder heroHolder = hero.GetHeroHolder();
        Collider2D[] collider2D = Physics2D.OverlapCircleAll(
            heroHolder.transform.position,heroData.atkRange.GetValue(),hero.monsterLayer);

        int count = 0;
        foreach (var collider in collider2D)
        {
            GameObject go = Instantiate(heroData.skillData.skillPrefab,
            collider.transform.position + new Vector3(0, 8, 0), Quaternion.identity);
            go.GetComponent<BaseProjectile>().Init(collider.transform, hero);
            count++;
            if (count == heroData.skillData.targetCount) break;
        }
    }

}

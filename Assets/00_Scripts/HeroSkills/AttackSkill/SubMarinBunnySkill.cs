using UnityEngine;

public class SubMarinBunnySkill : AttackSkillBase
{


    protected override void SkillLogic()
    {

        HeroHolder heroHolder = hero.GetHeroHolder();
        Collider2D[] enemiesInRange =
                Physics2D.OverlapCircleAll(heroHolder.transform.position, heroData.atkRange.GetValue(), hero.monsterLayer);

        Debug.Log("È£Ãâ");
        if (enemiesInRange.Length > 0)
        {
            Transform target = enemiesInRange[0].transform;
            if (target != null)
            {
                GameObject go = Instantiate(heroData.skillData.skillPrefab,
                    transform.position + new Vector3(0, 6, 0), Quaternion.identity);
                go.GetComponent<BaseProjectile>().Init(target.transform, hero);
            }
        }
    }
}

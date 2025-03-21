using UnityEngine;

public class DoctorBunnySkill : AttackSkillBase
{
    protected override void ActiveSkill()
    {

        HeroHolder heroHolder = hero.GetHeroHolder();
        Collider2D[] enemiesInRange =
                Physics2D.OverlapCircleAll(heroHolder.transform.position, heroData.atkRange.GetValue(), hero.monsterLayer);

        int currentTargetCount = 0;

        if (enemiesInRange.Length > 0)
        {
            foreach (var enemy in enemiesInRange)
            {
                if (currentTargetCount >= heroData.skillData.targetCount) break;
                if (enemy != null)
                {
                    GameObject go = Instantiate(heroData.skillData.skillPrefab,
                        transform.position, Quaternion.identity);
                    go.GetComponent<BaseProjectile>().Init(enemy.transform, hero);
                    currentTargetCount++;
                }
            }    
        }
    }

}

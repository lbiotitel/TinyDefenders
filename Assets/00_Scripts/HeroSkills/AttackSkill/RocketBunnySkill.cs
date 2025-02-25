using System.Collections;
using UnityEngine;

public class RocketBunnySkill : AttackSkillBase
{
    protected override void ActiveSkill()
    {
        StartCoroutine(RapidFire());
    }

    private IEnumerator RapidFire()
    { 
        for(int i = 0; i < 10; i++)
        {
            GameObject go = Instantiate(heroData.skillData.skillPrefab,
            transform.position, Quaternion.identity);
            go.GetComponent<Projectile>().Init(hero.target.transform, hero);
            yield return new WaitForSeconds(0.2f);
        }
    }
}

using UnityEngine;

public class SubMarinBunnySkill : AttackSkillBase
{


    protected override void ActiveSkill()
    {
        GameObject go = Instantiate(heroData.skillData.skillPrefab,
            transform.position + new Vector3(0,6,0), Quaternion.identity);
        go.GetComponent<Projectile>().Init(hero.target.transform, hero);
    }
}

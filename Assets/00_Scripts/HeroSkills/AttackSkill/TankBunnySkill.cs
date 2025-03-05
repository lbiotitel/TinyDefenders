using UnityEngine;

public class TankBunnySkill : AttackSkillBase
{
    protected override void ActiveSkill()
    {
        GameObject go = Instantiate(heroData.skillData.skillPrefab,
        transform.position, Quaternion.identity);
        go.GetComponent<BaseProjectile>().Init(hero.target.transform, hero);
    }
}

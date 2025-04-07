using System.Threading;
using UnityEngine;

public class DwarfArmoredBearRiderSkill : AttackSkillBase
{
    public override void SkillLogic()
    {
        base.SkillLogic();
        GameObject go = PoolingManager.instance.GetObject(heroData.skillData.skillPrefab, transform.position);
        go.GetComponent<BaseProjectile>().Init(null, hero, heroData.GetHeroSkillDamage());
    }

    public override bool CheckSkillCanActive()
    {
        return true;
    }
}

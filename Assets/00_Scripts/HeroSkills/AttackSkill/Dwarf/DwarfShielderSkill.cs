using System.Collections.Generic;
using UnityEngine;

public class DwarfShielderSkill : AttackSkillBase
{

    public override void SkillLogic()
    {
        base.SkillLogic();

        List<Monster> monsterList = FindCloseMonster(heroData.skillData.targetCount);
        GameObject go = PoolingManager.instance.GetObject(heroData.skillData.skillPrefab, transform.position);

        foreach (var monster in monsterList)
        {
            NetworkRpcManager.instance.
                AttackMonster(NetUtils.GetPlayerNetworkType(),heroData.GetHeroSkillDamage(),hero,monster,heroData.skillDebuffEffects);
        }
    }
}

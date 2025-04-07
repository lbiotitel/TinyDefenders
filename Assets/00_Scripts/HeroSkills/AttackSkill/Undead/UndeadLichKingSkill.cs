using System.Collections.Generic;
using UnityEngine;

public class UndeadLichKingSkill : AttackSkillBase
{
    public override void SkillLogic()
    {
        base.SkillLogic();
        List<Monster> monsterList = FindCloseMonster(heroData.skillData.targetCount);

        foreach (var monster in monsterList)
        {
            if (monster.IsDead()) continue;
            PoolingManager.instance.GetObject(heroData.skillData.skillPrefab, transform.position);
            NetworkRpcManager.instance.AttackMonster(NetUtils.GetPlayerNetworkType(),heroData.GetHeroSkillDamage(),hero,monster,heroData.skillDebuffEffects);
        }
    }
}

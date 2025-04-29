using System.Collections.Generic;

public class DwarfHunterSkill : AttackSkillBase
{
    public override void SkillLogic()
    {
        base.SkillLogic();

        int targetCount = skillData.targetCount;

        List<Monster> monsterList = FindCloseMonster(targetCount);

        if (monsterList.Count > 0)
        {
            Monster monster = monsterList[0];
            WideAreaProjectile projectile =
                PoolingManager.instance.GetObject<WideAreaProjectile>(skillData.skillPrefab, transform.position);

            projectile.Init(monster, hero, heroData.GetHeroSkillDamage());
            projectile.TargetCount = targetCount;
        }
    }
}

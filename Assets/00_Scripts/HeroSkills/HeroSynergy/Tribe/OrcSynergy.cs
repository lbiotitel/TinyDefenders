using UnityEngine;

public class OrcSynergy : HeroSynergyBase
{
    [SerializeField] private float[] criticalRate;
    [SerializeField] private float excutionHpPercent;

    public override void HeroSynergyActive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.AddCriticalRateModifire(playerNetworkType, criticalRate[level - 1]);
        if (level == 3)
        {
            EventManager.instance.onHeroAttackToMonsterAction += OrcExcution;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.RemoveCriticalRateModifire(playerNetworkType, criticalRate[level - 1]);
        if (level == 3)
        {
            EventManager.instance.onHeroAttackToMonsterAction -= OrcExcution;
        }
    }

    private void OrcExcution(Hero hero, Monster monster)
    {
        if (monster.IsDead()) return;
        double hpPercent = (monster.hp / monster.maxHp) * 100f;
        Debug.Log(hpPercent);
        if (excutionHpPercent >= hpPercent)
        {
            double damage = monster.hp;
            monster.ExcutionMonster(playerNetworkType,hero);
        }
    }
}

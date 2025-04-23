using UnityEngine;

public class WarriorSynergy : HeroSynergyBase
{
    [SerializeField] private float[] extraHitValue;
    [SerializeField] private float[] extraHitProbability;

    public override void HeroSynergyActive(int level)
    {
        CheckSynergyLevel(level);
        if (currentSynergyLevel == 1)
        {
            EventManager.instance.onHeroAttackToMonsterAction += WarriorSynergyLogic;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        CheckSynergyLevel(level);
        if (currentSynergyLevel == 1)
        {
            EventManager.instance.onHeroAttackToMonsterAction -= WarriorSynergyLogic;
        }
    }

    private void WarriorSynergyLogic(Hero hero, Monster monster)
    {
        if (hero.GetHeroData().heroClass != HeroClass.WARRIOR) return;

        MathUtils.RandomChanceAction(extraHitProbability[currentSynergyLevel],() => {
            
            double damage = hero.GetHeroData().GetHeroAtk() * (extraHitValue[currentSynergyLevel] / 100f);
            NetworkRpcManager.instance.AttackMonster(NetUtils.GetPlayerNetworkType(), damage, hero, monster, null, false);
        });
    }
}

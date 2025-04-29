using UnityEngine;

public class HunterSynergy : HeroSynergyBase
{
    [SerializeField] private float maxAtkRangeBonus;
    [SerializeField] private float atkRangeBonusDamage;
    [SerializeField] private float[] criDamageValue;

    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
        if (currentSynergyLevel == 3)
        {
            EventManager.instance.onMonsterBeforeTakeDamageAction += HunterSynergyLogic;
        }
        else {
            PlayerStatManager.instance.
                AddStatModifire(synergyNetworkType, HeroClass.HUNTER,PlayerStatType.CriticalDamage, criDamageValue[currentSynergyLevel]);
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
        if (currentSynergyLevel == 3)
        {
            EventManager.instance.onMonsterBeforeTakeDamageAction -= HunterSynergyLogic;
        }
        else
        {
            PlayerStatManager.instance.
                RemoveStatModifire(synergyNetworkType, HeroClass.HUNTER, PlayerStatType.CriticalDamage, criDamageValue[currentSynergyLevel]);
        }
    }

    private void HunterSynergyLogic(DamageEvent damageEvent)
    {
        if (damageEvent.hero.GetHeroData().heroClass != HeroClass.HUNTER) return;
        if (damageEvent.hero.heroOwnerNetworkType != synergyNetworkType) return;

        double finalDamage = damageEvent.finalDamage;

        Monster monster = damageEvent.monster;
        Hero hero = damageEvent.hero;

        float distance = Vector2.Distance(monster.transform.position, hero.transform.position);

        float bonusMultiplier = ((Mathf.Clamp(distance,0, maxAtkRangeBonus)) * atkRangeBonusDamage) / 100f;
        finalDamage += finalDamage * bonusMultiplier;

        damageEvent.finalDamage = finalDamage;
        Debug.Log(damageEvent.finalDamage);
    }
}

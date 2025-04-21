using UnityEngine;

public class HunterSynergy : HeroSynergyBase
{
    [SerializeField] private float maxAtkRangeBonus;
    [SerializeField] private float[] atkRangeBonusDamage;

    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
        if (currentSynergyLevel == 1)
        {
            EventManager.instance.onMonsterBeforeTakeDamageAction += HunterSynergyLogic;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
        if (currentSynergyLevel == 1)
        {
            EventManager.instance.onMonsterBeforeTakeDamageAction -= HunterSynergyLogic;
        }
    }

    private void HunterSynergyLogic(DamageEvent damageEvent)
    {
        if (damageEvent.hero.GetHeroData().heroClass != HeroClass.HUNTER) return;

        double finalDamage = damageEvent.finalDamage;

        Monster monster = damageEvent.monster;
        Hero hero = damageEvent.hero;

        float distance = Vector2.Distance(monster.transform.position, hero.transform.position);

        float bonusMultiplier = ((Mathf.Clamp(distance,0, maxAtkRangeBonus)) * atkRangeBonusDamage[currentSynergyLevel]) / 100f;
        finalDamage += finalDamage * bonusMultiplier;

        damageEvent.finalDamage = finalDamage;
        Debug.Log(damageEvent.finalDamage);
    }
}

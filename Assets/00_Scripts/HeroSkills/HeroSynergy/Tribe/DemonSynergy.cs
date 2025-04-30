using UnityEngine;

public class DemonSynergy : HeroSynergyBase
{
    [SerializeField] private float[] atkModifireValue;
    [SerializeField] private float extraDamage;
    [SerializeField] private int demonSynergyLevel = 4; 

    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;

        ModifyStat(true);

        if (currentSynergyLevel == demonSynergyLevel)
        {
            EventManager.instance.onHeroAttackToMonsterAction += DemonSynergyLogic;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (!CheckSynergyLevel(level)) return;

        ModifyStat(false);

        if (currentSynergyLevel == demonSynergyLevel)
        {
            EventManager.instance.onHeroAttackToMonsterAction -= DemonSynergyLogic;
        }
    }

    private void ModifyStat(bool isAdding)
    {
        float modifierValue = GetCurrentModifierValue();
        if (isAdding)
        {
            PlayerStatManager.instance.AddStatModifire(
                synergyNetworkType,
                HeroTribe.DEMON,
                PlayerStatType.Atk,
                modifierValue
            );
        }
        else
        {
            PlayerStatManager.instance.RemoveStatModifire(
                synergyNetworkType,
                HeroTribe.DEMON,
                PlayerStatType.Atk,
                modifierValue
            );
        }
    }

    private float GetCurrentModifierValue()
    {
        return atkModifireValue[Mathf.Clamp(currentSynergyLevel, 0, atkModifireValue.Length - 1)];
    }


    private void DemonSynergyLogic(Hero hero, Monster monster)
    {
        if (!IsValidDemonSynergy(hero, monster)) return;

        double heroAtk = hero.GetHeroData().GetHeroAtk();
        double damage = heroAtk + (heroAtk * extraDamage);

        NetworkRpcManager.instance.AttackMonster(synergyNetworkType,damage,hero,monster,null,false);
    }

    private bool IsValidDemonSynergy(Hero hero, Monster monster)
    {
        return hero.GetHeroData().heroTribe == HeroTribe.DEMON &&
               hero.heroOwnerNetworkType == synergyNetworkType &&
               !monster.IsIgnite;
    }
}
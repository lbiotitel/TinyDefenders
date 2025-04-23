using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DebuffEffect
{
    public DebuffType debuffType;
    public float[] values;
}

public enum HeroStatType 
{
    Atk,
    AtkSpeed,
    AtkRange,
    CriticalRate,
    CriticalDamage,
    SkillAcc
}


[System.Serializable]
public class SkillData
{
    public float coolTime;
    public GameObject skillPrefab;

    [Header("Attack Skill")]
    public float skillDamage;
    public int targetCount;
    public float skillDuration;

    [Header("Buff Skill")]
    public float buffTime;
    public float buffAmount;
    public float buffRange;

}



[CreateAssetMenu(fileName = "HeroScriptable", menuName = "Scriptable Objects/HeroScriptable")]
public class HeroScriptable : ScriptableObject
{
    [Header("Stat")]
    public int heroLevel = 1;
    public string heroName;
    public HeroRarity rarity;
    public int heroCost;
    public Stat atk;
    public Stat damage;
    public Stat atkSpeed;
    public Stat atkRange;
    public Stat skillAcc;
    public Stat criticalRate;
    public Stat criticalDamage;
    private const float baseCriticalDamage = 120;

    [TextArea] public string heroDesc;
    private PlayerNetworkType heroNetworkType;

    [Header("Extra")]
    public HeroTribe heroTribe;
    public HeroClass heroClass;
    public Sprite heroMainImage;
    public RuntimeAnimatorController runtimeAnimatorController;

    [Header("Projectile")]
    public BaseProjectile projectile = null;

    [Header("Debuff")]
    public List<DebuffEffect> debuffEffects;


    [Header("Skill Debuff")]
    public List<DebuffEffect> skillDebuffEffects;

    [Header("Skill")]
    public SkillData skillData;

    private Hero hero;

    private void OnValidate()
    {
        heroName = this.name;
    }

    public void InitHeroNetworkType(PlayerNetworkType playerNetworkType, Hero hero)
    {
        heroNetworkType = playerNetworkType;
        this.hero = hero;
        criticalDamage.AddModifire(baseCriticalDamage);

        EventManager.instance.onHeroAddStatModifireAction += AddStatModifire;
        EventManager.instance.onHeroRemoveStatModifireAction += RemoveStatModifire;
    }

    public string GetLocalizationName()
    {
        return Utils.Localization_Text(Localize.Hero, heroName);
    }

    public string GetLocalizationDesc()
    {
        /*string desc = Utils.Localization_Text(Localize.Hero, heroName + "Desc");

        if (!string.IsNullOrEmpty(desc))
        {
            return desc;
        }*/

        return "";
    }



    #region Hero Stat

    #region Stat Modify

    public void AddStatModifire(HeroStatType statType, float value) => NetworkRpcManager.instance.AddHeroStatModifireServerRpc((int)statType, hero.NetworkObjectId, value);

    private void AddStatModifire(Hero targetHero, HeroStatType statType, float value)
    {
        if (hero != targetHero) return;

        switch (statType)
        {
            case HeroStatType.Atk:
                atk.AddModifire(value);
                break;
            case HeroStatType.AtkSpeed:
                atkSpeed.AddModifire(value);
                break;
            case HeroStatType.AtkRange:
                atkRange.AddModifire(value);
                break;
            case HeroStatType.CriticalRate:
                criticalRate.AddModifire(value);
                break;
            case HeroStatType.CriticalDamage:
                criticalDamage.AddModifire(value);
                break;
            case HeroStatType.SkillAcc:
                skillAcc.AddModifire(value);
                break;
        }
    }
    public void RemoveStatModifire(HeroStatType statType, float value) => NetworkRpcManager.instance.RemoveHeroStatModifireServerRpc((int)statType, hero.NetworkObjectId, value);

    private void RemoveStatModifire(Hero targetHero, HeroStatType statType, float value)
    {
        if (hero != targetHero) return;

        switch (statType)
        {
            case HeroStatType.Atk:
                atk.RemoveModifire(value);
                break;
            case HeroStatType.AtkSpeed:
                atkSpeed.RemoveModifire(value);
                break;
            case HeroStatType.AtkRange:
                atkRange.RemoveModifire(value);
                break;
            case HeroStatType.CriticalRate:
                criticalRate.RemoveModifire(value);
                break;
            case HeroStatType.CriticalDamage:
                criticalDamage.RemoveModifire(value);
                break;
            case HeroStatType.SkillAcc:
                skillAcc.RemoveModifire(value);
                break;
        }
    }

    #endregion


    #region About Hero Atk
    public double GetHeroAtk()
    {
        double atk = this.atk.GetValue();
        atk = atk + (atk * (PlayerStatManager.instance.GetBonusAtk(heroNetworkType) / 100f));
        return atk;
    }

    public double GetHeroBaseAtk()
    {
        return atk.baseValue;
    }

    public double GetHeroBonusAtk()
    {
        return GetHeroAtk() - GetHeroBaseAtk();
    }
    #endregion


    #region About Hero AtkSpeed


    public float GetHeroAtkSpeed()
    {
        float modifire = PlayerStatManager.instance.GetBonusAtkSpeed(heroNetworkType) + this.atkSpeed.GetModifireValue();
        float atkSpeed = GetHeroBaseAtkSpeed() + (GetHeroBaseAtkSpeed() * (modifire / 100f));
        return atkSpeed;
    }

    public float GetHeroBaseAtkSpeed()
    {
        return atkSpeed.baseValue;
    }

    public float GetHeroBounusAtkSpeed()
    {
        return GetHeroAtkSpeed() - GetHeroBaseAtkSpeed();
    }
    #endregion


    #region About Hero AtkRange
    public float GetHeroBaseAtkRange()
    {
        return atkRange.baseValue;
    }

    public float GetHeroAtkRange()
    {
        float atkRange = GetHeroBaseAtkRange() + this.atkRange.GetModifireValue() + PlayerStatManager.instance.GetBonusAtkRange(heroNetworkType);
        return atkRange;
    }

    public float GetHeroBonusAtkRange()
    {
        return GetHeroAtkRange() - GetHeroBaseAtkRange();
    }

    #endregion


    public double GetHeroSkillDamage()
    {
        double damage = GetHeroAtk() * (skillData.skillDamage / 100f);
        return damage;
    }

    public float GetHeroCriDamage()
    {
        float criDamage = criticalDamage.GetModifireValue() + PlayerStatManager.instance.GetBonusCriDamage(heroNetworkType);
        return criDamage / 100f;
    }

    #region About Hero Critical Rate

    public float GetHeroBaseCriticalRate()
    {
        return criticalRate.baseValue;
    }

    public float GetHeroBonusCriticalRate()
    {
        return GetHeroCriticalRate() - GetHeroBaseCriticalRate();
    }

    public float GetHeroCriticalRate()
    {
        return GetHeroBaseCriticalRate() + criticalRate.GetModifireValue() +
            PlayerStatManager.instance.GetBonusCriticalRate(heroNetworkType);
    }

    #endregion


    public float GetHeroSkillAcc()
    {
        return skillAcc.GetModifireValue() + PlayerStatManager.instance.GetBonusSkillAcc(heroNetworkType);
    }

    #endregion


    private void OnDestroy()
    {
        EventManager.instance.onHeroAddStatModifireAction -= AddStatModifire;
        EventManager.instance.onHeroRemoveStatModifireAction -= RemoveStatModifire;
    }
}

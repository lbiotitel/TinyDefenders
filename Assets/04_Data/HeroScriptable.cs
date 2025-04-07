using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DebuffEffect
{
    public DebuffType debuffType;
    public float[] values;
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
    [TextArea] public string heroDesc;
    private PlayerNetworkType heroNetworkType;

    [Header("Extra")]
    public HeroTribe heroTribe;
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

    private void OnEnable()
    {
        heroName = this.name;
    }

    public void InitHeroNetworkType(PlayerNetworkType playerNetworkType)
    {
        heroNetworkType = playerNetworkType;
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

    #region About Hero Atk
    public double GetHeroAtk()
    {
        double atk = this.atk.GetValue();
        atk = atk + (atk * (PlayerStatManager.instance.GetBonusAtk(NetUtils.GetPlayerNetworkType()) / 100f));
        atk += atk * (PlayerStatManager.instance.GetHeroUpgradeLevel(heroTribe, heroNetworkType) * 0.05f);
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
        float synergyStat = PlayerStatManager.instance.GetBonusAtkSpeed(NetUtils.GetPlayerNetworkType()) + this.atkSpeed.GetModifireValue();
        float atkSpeed = GetHeroBaseAtkSpeed() + (GetHeroBaseAtkSpeed() * (synergyStat / 100f));
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
        return atkRange.GetValue();
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
        return criticalDamage.GetValue() / 100f;
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
            PlayerStatManager.instance.GetBonusCriticalRate(NetUtils.GetPlayerNetworkType());
    }

    #endregion


    public float GetHeroSkillAcc()
    {
        return skillAcc.GetModifireValue() + PlayerStatManager.instance.GetBonusSkillAcc(heroNetworkType);
    }

    public void AddAtkModifire(float value)
    {
        atk.AddModifire(value);
    }

    public void RemovAtkModifire(float value)
    {
        atk.RemoveModifire(value);
    }


    #endregion
}

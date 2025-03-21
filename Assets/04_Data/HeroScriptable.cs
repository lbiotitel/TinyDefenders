using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;

[System.Serializable]
public class DebuffEffect { 
    public DebuffType debuffType;
    public float[] values;
}


[System.Serializable]
public class SkillData {
    public float coolTime;
    public GameObject skillPrefab;

    [Header("Attack Skill")]
    public float skillDamage;
    public int targetCount;

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
    public Stat atk;
    public Stat damage;
    public Stat criticalRate;
    public Stat atkSpeed;
    public Stat atkRange;
    [TextArea] public string heroDesc;
    
    [Header("Extra")]
    public Sprite heroMainImage;
    public RuntimeAnimatorController runtimeAnimatorController;

    [Header("Projectile")]
    public BaseProjectile projectile = null;

    [Header("Debuff")]
    public List<DebuffEffect> debuffEffects;

    [Header("Skill")]
    public SkillData skillData;

    private void OnEnable()
    {
        heroName = this.name;
    }

}

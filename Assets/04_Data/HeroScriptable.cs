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


[System.Serializable]
public struct HeroData : INetworkSerializable
{
    public string heroName;
    public int heroAtk;
    public float heroAtkSpeed;
    public float heroAtkRange;
    public string rarity;


    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref heroName);
        serializer.SerializeValue(ref heroAtk);
        serializer.SerializeValue(ref heroAtkSpeed);
        serializer.SerializeValue(ref heroAtkRange);
        serializer.SerializeValue(ref rarity);
    }
}

[CreateAssetMenu(fileName = "HeroScriptable", menuName = "Scriptable Objects/HeroScriptable")]
public class HeroScriptable : ScriptableObject
{
    [Header("Stat")]
    public string heroName;
    public int atk;
    public float atkSpeed;
    public float atkRange;
    public Rarity rarity;

    [Header("Extra")]
    public Sprite heroMainImage;
    public GameObject leftEquipmentPrefab = null;
    public GameObject rightEquipmentPrefab = null;
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

    public HeroData GetHeroData()
    {
        return new HeroData
        {
            heroName = heroName,
            heroAtk = atk,
            heroAtkSpeed = atkSpeed,
            heroAtkRange = atkRange,
            rarity = rarity.ToString(),
        };
    }
}

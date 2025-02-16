using Unity.Netcode;
using UnityEngine;


[System.Serializable]
public struct HeroData : INetworkSerializable
{
    public string heroName;
    public int heroAtk;
    public float heroAtkSpeed;
    public float heroAtkRange;
    public string leftEquipmentName;
    public string rightEquipmentName;
    public string rarity;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref heroName);
        serializer.SerializeValue(ref heroAtk);
        serializer.SerializeValue(ref heroAtkSpeed);
        serializer.SerializeValue(ref heroAtkRange);
        serializer.SerializeValue(ref leftEquipmentName);
        serializer.SerializeValue(ref rightEquipmentName);
        serializer.SerializeValue(ref rarity);
    }
}

[CreateAssetMenu(fileName = "HeroScriptable", menuName = "Scriptable Objects/HeroScriptable")]
public class HeroScriptable : ScriptableObject
{
    public string heroName;
    public int atk;
    public float atkSpeed;
    public float atkRange;
    public Rarity rarity;

    public string leftEquipmentName = string.Empty;
    public string rightEquipmentName = string.Empty;

    public RuntimeAnimatorController runtimeAnimatorController;

    public HeroData GetHeroData()
    {
        return new HeroData
        {
            heroName = heroName,
            heroAtk = atk,
            heroAtkSpeed = atkSpeed,
            heroAtkRange = atkRange,
            leftEquipmentName = leftEquipmentName,
            rightEquipmentName = rightEquipmentName,
            rarity = rarity.ToString(),
        };
    }
}

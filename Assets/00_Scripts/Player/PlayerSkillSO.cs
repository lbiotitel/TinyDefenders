using UnityEngine;
using UnityEngine.Experimental.AI;

[CreateAssetMenu(fileName = "PlayerSkillSO", menuName = "Scriptable Objects/PlayerSkillSO")]
public class PlayerSkillSO : ScriptableObject
{
    public PlayerSkillType playerSkillType;
    public string skillName;
    public string[] skillDesc;
    public int skillLevel = 0;
    public int skillMaxLevel;
    public int passiveSkillLevel = 0;
    public Sprite[] passiveImages; 

    private bool[] passiveSkillUnlock = { false,false, false };
    public PlayerSkillBase[] playerSkillBases;

    public bool AddSkillPoint()
    {
        if (skillLevel >= skillMaxLevel)
            return false;
        skillLevel++;
        return true;
    }

    public bool SubtractSkillPoint()
    {
        if (skillLevel <= 0) return false;
        skillLevel--;
        return true;
    }
}

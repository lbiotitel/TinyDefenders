using UnityEngine;

[CreateAssetMenu(fileName = "MonsterScriptable", menuName = "Scriptable Objects/MonsterScriptable")]
public class MonsterScriptable : ScriptableObject
{
    public string monsterName;
    public double monsterHp;
    public float monsterSpeed;
    public MonsterType monsterType;
    public RuntimeAnimatorController animatorController;

    private void OnEnable()
    {
        monsterName = name;
    }
}

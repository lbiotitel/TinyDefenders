using UnityEngine;

[CreateAssetMenu(fileName = "MonsterScriptable", menuName = "Scriptable Objects/MonsterScriptable")]
public class MonsterScriptable : ScriptableObject
{
    public string monsterName;
    public double monsterHp;
    public float monsterSpeed;
    public RuntimeAnimatorController animatorController;

    private void OnEnable()
    {
        monsterName = name;
    }
}

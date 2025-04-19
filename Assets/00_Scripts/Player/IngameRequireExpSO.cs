using UnityEngine;

[CreateAssetMenu(fileName = "IngameRequireExpSO", menuName = "Scriptable Objects/IngameRequireExpSO")]
public class IngameRequireExpSO : ScriptableObject
{
    [SerializeField] private int[] requireExp;


    public int GetRequireExp(int level)
    { 
        level = Mathf.Clamp(level, 0,requireExp.Length - 1);
        return requireExp[level];
    }
}

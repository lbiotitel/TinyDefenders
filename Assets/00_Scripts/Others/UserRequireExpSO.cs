using UnityEngine;

[CreateAssetMenu(fileName = "UserLevelSO", menuName = "Scriptable Objects/UserLevelSO")]
public class UserRequireExpSO : ScriptableObject
{
    public int[] requireUserExp;

    public int GetUserRequireExp(int level)
    {
        if (level < 0 || level >= requireUserExp.Length)
        {
            Debug.LogError("레벨 범위 벗어남");
            return 0;
        }
        return requireUserExp[level];
    }
}

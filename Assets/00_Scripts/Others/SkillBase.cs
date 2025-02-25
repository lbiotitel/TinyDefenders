using UnityEngine;

public class SkillBase : ScriptableObject
{
    public bool skillDelay = false;
    public float skillCoolTime = 0f;


    public virtual void ActiveSkill() { }

    
}

using UnityEngine;

public class AttackSkillBase : Skill
{ 

    protected override void SkillLogic()
    {
        if (!skillDelay)
        {
            skillDelay = true;
            StartCoroutine(SkillDelayCoroutine());
            ActiveSkill();
        }
    }

    protected virtual void ActiveSkill()
    {
        
    }
}

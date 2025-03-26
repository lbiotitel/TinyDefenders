using UnityEngine;

public class PassiveSkillBase : SkillBase
{
    protected override void ActiveSkill()
    {
        if (!skillDelay)
        {
            if (IsServer)
            {
                skillDelay = true;
                StartCoroutine(SkillDelayCoroutine());
            }
            SkillLogic();
        }
    }

    protected virtual void SkillLogic()
    {

    }
}

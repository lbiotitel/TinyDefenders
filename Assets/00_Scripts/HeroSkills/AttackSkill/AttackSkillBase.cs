using UnityEngine;

public class AttackSkillBase : Skill
{ 

    protected override void SkillLogic()
    {
        if (hero.target == null || hero.target.GetComponent<Monster>().IsDead()) return;
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

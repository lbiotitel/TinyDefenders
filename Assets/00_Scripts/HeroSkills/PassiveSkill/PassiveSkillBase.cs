using UnityEngine;

public class PassiveSkillBase : SkillBase
{
    public override bool CheckSkillCanActive()
    {
        return true;
    }

    public override void SkillLogic()
    {
        hero.AnimatorChange("SKILL", true);
    }
}

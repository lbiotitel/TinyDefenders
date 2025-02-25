using UnityEngine;

public class LobberBunnySkill : PassiveSkillBase
{
    protected override void ActiveSkill()
    {
        GameManager.instance.GetGold(2, HostType.CLIENT);
    }
}

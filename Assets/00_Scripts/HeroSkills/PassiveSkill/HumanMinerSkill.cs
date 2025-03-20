using UnityEngine;

public class HumanMinerSkill : PassiveSkillBase
{
    protected override void ActiveSkill()
    {
        GameManager.instance.GetGold(2, HostType.CLIENT);
        Instantiate(heroData.skillData.skillPrefab,transform.position,Quaternion.identity);
    }
}

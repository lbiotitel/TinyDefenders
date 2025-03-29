using UnityEngine;

public class HumanSynergy : HeroSynergyBase
{
    [SerializeField] private float[] skillAcc;
    [SerializeField] private float skillResetRate;

    //2 4 레벨은 스킬포인트 회복속도만 증가 3렙은 스킬포인트 회복하고 스킬 초기화 확률 20%

    public override void HeroSynergyActive(int level)
    {
        Debug.Log(playerNetworkType);
        if (level < 1) return;
        PlayerStatManager.instance.AddSkillAccModifire(playerNetworkType, skillAcc[level - 1]);
        if (level == 3)
        {
            EventManager.instance.onHeroUseSkillAction += HeroCoolTimeReset;
        }
    }

    private void HeroCoolTimeReset(Hero hero)
    {
        float randomValue = Random.Range(0,100f);
        if (randomValue > skillResetRate) return;

        Debug.Log("스킬 사용");

        if (hero.TryGetComponent(out SkillContainer skillContainer))
        {
            skillContainer.ResetSkill();
            Debug.Log("스킬 초기화");
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.RemoveSkillAccModifire(playerNetworkType, skillAcc[level - 1]);
        if (level == 3)
        {
            EventManager.instance.onHeroUseSkillAction -= HeroCoolTimeReset;
        }
    }
}

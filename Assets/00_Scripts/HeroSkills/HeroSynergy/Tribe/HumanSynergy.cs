using UnityEngine;

public class HumanSynergy : HeroSynergyBase
{
    [SerializeField] private float[] skillAcc;
    [SerializeField] private float skillResetRate;

    //2 4 레벨은 스킬포인트 회복속도만 증가 3렙은 스킬포인트 회복하고 스킬 초기화 확률 20%

    public override void HeroSynergyActive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.AddSkillAccModifire(synergyNetworkType, skillAcc[level - 1]);
        if (level == 3)
        {
            EventManager.instance.onHeroUseSkillAction += HeroUseTwiceSkill;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.RemoveSkillAccModifire(synergyNetworkType, skillAcc[level - 1]);
        if (level == 3)
        {
            EventManager.instance.onHeroUseSkillAction -= HeroUseTwiceSkill;
        }
    }


    private void HeroUseTwiceSkill(Hero hero)
    {
        float randomValue = Random.Range(0, 100f);
        if (randomValue > skillResetRate) return;

        if (hero.TryGetComponent(out SkillContainer skillContainer))
        {
            skillContainer.ResetSkill();
        }
    }

}

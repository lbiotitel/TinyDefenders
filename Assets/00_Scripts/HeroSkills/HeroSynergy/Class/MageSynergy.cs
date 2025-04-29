using UnityEngine;

public class MageSynergy : HeroSynergyBase
{

    [SerializeField] private float[] skillAcc;
    [SerializeField] private float extraSkillProbability;

    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
        PlayerStatManager.instance.AddStatModifire(NetUtils.GetPlayerNetworkType(), HeroClass.MAGE, PlayerStatType.SkillAcc, skillAcc[currentSynergyLevel]);
        if (currentSynergyLevel == 3)
        {
            EventManager.instance.onHeroUseSkillAction += MageSynergyLogic;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
        PlayerStatManager.instance.RemoveStatModifire(NetUtils.GetPlayerNetworkType(), HeroClass.MAGE, PlayerStatType.SkillAcc, skillAcc[currentSynergyLevel]);
        if (currentSynergyLevel == 3)
        {
            EventManager.instance.onHeroUseSkillAction -= MageSynergyLogic;
        }
    }

    private void MageSynergyLogic(Hero hero)
    {
        if (hero.GetHeroData().heroClass != HeroClass.MAGE) return;
        if (hero.heroOwnerNetworkType != synergyNetworkType) return;

        MathUtils.RandomChanceAction(extraSkillProbability, () =>
        {
            if (hero.TryGetComponent(out SkillContainer skillContainer))
            {
                skillContainer.ResetSkill();
            }
        });
    }
}

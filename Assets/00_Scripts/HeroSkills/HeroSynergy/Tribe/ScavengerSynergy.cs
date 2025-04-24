using UnityEngine;

public class ScavengerSynergy : HeroSynergyBase
{
    [SerializeField] private int[] goldBonus;
    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;

        if (currentSynergyLevel == 1)
        { 
            EventManager.instance.onWaveFinishAction += ScavengerSynergyLogic;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (!CheckSynergyLevel(level)) return;

        if (level == 1)
        {
            EventManager.instance.onWaveFinishAction -= ScavengerSynergyLogic;
        }
    }

    private void ScavengerSynergyLogic()
    {
        GameManager.instance.GetGold(goldBonus[currentSynergyLevel],synergyNetworkType);
    }
}

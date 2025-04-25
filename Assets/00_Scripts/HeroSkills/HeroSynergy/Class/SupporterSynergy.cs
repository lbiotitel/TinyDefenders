using UnityEngine;

public class SupporterSynergy : HeroSynergyBase
{
    [SerializeField] private float[] buffModifireValue;
    [SerializeField] private float buffRangeModifireValue;

    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;

        if (currentSynergyLevel < 3)
        {
            PlayerStatManager.instance.
                AddStatModifire(synergyNetworkType, PlayerStatType.BuffBonus, buffModifireValue[currentSynergyLevel]);
        }
        else if (currentSynergyLevel == 3)
        {
            PlayerStatManager.instance.
               AddStatModifire(synergyNetworkType, PlayerStatType.BuffRangeBonus, buffRangeModifireValue);
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (!CheckSynergyLevel(level)) return;

        if (currentSynergyLevel < 3)
        {
            PlayerStatManager.instance.
                RemoveStatModifire(synergyNetworkType, PlayerStatType.BuffBonus, buffModifireValue[currentSynergyLevel]);
        }
        else if (currentSynergyLevel == 3)
        {
            PlayerStatManager.instance.
                RemoveStatModifire(synergyNetworkType, PlayerStatType.BuffRangeBonus, buffRangeModifireValue);
        }
    }

}

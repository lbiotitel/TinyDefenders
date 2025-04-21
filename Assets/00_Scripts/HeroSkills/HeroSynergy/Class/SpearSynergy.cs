using UnityEngine;

public class SpearSynergy : HeroSynergyBase
{
    [SerializeField] private float[] atkRangeModifireValue;
    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
        PlayerStatManager.instance.AddAtkRangeModifire(playerNetworkType, atkRangeModifireValue[currentSynergyLevel]);
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
        PlayerStatManager.instance.RemoveAtkRangeModifire(playerNetworkType, atkRangeModifireValue[currentSynergyLevel]);
    }
}

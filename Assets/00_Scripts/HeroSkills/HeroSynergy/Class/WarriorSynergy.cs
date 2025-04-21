using UnityEngine;

public class WarriorSynergy : HeroSynergyBase
{
    [SerializeField] private float[] atkModifireValue;

    public override void HeroSynergyActive(int level)
    {
        CheckSynergyLevel(level);
        PlayerStatManager.instance.AddAtkModifire(playerNetworkType, atkModifireValue[currentSynergyLevel]);

    }

    public override void HeroSynergyDeactive(int level)
    {
        CheckSynergyLevel(level);
        PlayerStatManager.instance.RemoveAtkModifire(playerNetworkType, atkModifireValue[currentSynergyLevel]);

    }
}

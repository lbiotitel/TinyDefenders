using UnityEngine;

public class ElfSynergy : HeroSynergyBase
{
    [SerializeField] private float[] atkSpeed;

    public override void HeroSynergyActive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.AddAtkSpeedModifire(playerNetworkType, atkSpeed[level - 1]);
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.RemoveAtkSpeedModifire(playerNetworkType, atkSpeed[level - 1]);
    }
}

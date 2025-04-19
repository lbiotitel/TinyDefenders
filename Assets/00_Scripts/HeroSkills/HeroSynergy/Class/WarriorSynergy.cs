using UnityEngine;

public class WarriorSynergy : HeroSynergyBase
{
    [SerializeField] private float[] atkModifireValue;

    public override void HeroSynergyActive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.AddAtkModifire(playerNetworkType, atkModifireValue[level - 1]);

    }

    public override void HeroSynergyDeactive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.RemoveAtkModifire(playerNetworkType, atkModifireValue[level - 1]);

    }
}

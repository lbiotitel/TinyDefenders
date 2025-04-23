using UnityEngine;

public class RelicMaskOfRage : RelicBase
{
    [SerializeField] private int increaseAmount;
    public override void ActiveRelic()
    {
        PlayerStatManager.instance.AddStatModifire(playerNetworkType, PlayerStatType.CriticalDamage, increaseAmount);
    }
}

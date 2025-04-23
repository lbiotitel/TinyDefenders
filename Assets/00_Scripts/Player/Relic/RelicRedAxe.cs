using UnityEngine;

public class RelicRedAxe : RelicBase
{
    [SerializeField] private int increaseAmount;
    public override void ActiveRelic()
    {
        PlayerStatManager.instance.AddStatModifire(playerNetworkType, PlayerStatType.CriticalRate, increaseAmount);
    }
}

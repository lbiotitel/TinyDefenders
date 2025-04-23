using UnityEngine;

public class RelicBluntSword : RelicBase
{
    [SerializeField] private int increaseAmount;

    public override void ActiveRelic()
    {
        PlayerStatManager.instance.AddStatModifire(playerNetworkType, PlayerStatType.Atk, increaseAmount);
    }
}

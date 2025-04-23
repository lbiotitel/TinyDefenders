using UnityEngine;

public class RelicBluntBow : RelicBase
{
    [SerializeField] private int increaseAmount;
    public override void ActiveRelic()
    {
        PlayerStatManager.instance.AddStatModifire(playerNetworkType, PlayerStatType.AtkSpeed, increaseAmount);
    }
}

using UnityEngine;

public class RelicGlasses : RelicBase
{
    [SerializeField] private float increaseAmount;

    public override void ActiveRelic()
    {
        PlayerStatManager.instance.AddStatModifire(playerNetworkType, PlayerStatType.AtkRange, increaseAmount);
    }
}

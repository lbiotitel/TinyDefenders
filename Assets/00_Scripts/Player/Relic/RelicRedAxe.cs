using UnityEngine;

public class RelicRedAxe : RelicBase
{
    [SerializeField] private int increaseAmount;
    public override void ActiveRelic()
    {
        if (playerNetworkType == PlayerNetworkType.CLIENT)
        {
            NetworkRpcManager.instance.AddPlayerCriticalRateModifireServerRpc((int)playerNetworkType, increaseAmount);
        }
        PlayerStatManager.instance.AddCriticalRateModifire(playerNetworkType, increaseAmount);
    }
}

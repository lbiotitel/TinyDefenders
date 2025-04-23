using UnityEngine;

public class RelicGlasses : RelicBase
{
    [SerializeField] private float increaseAmount;

    public override void ActiveRelic()
    {
        if (playerNetworkType == PlayerNetworkType.CLIENT)
        {
            NetworkRpcManager.instance.AddPlayerAtkRangeServerRpc((int)playerNetworkType, increaseAmount);
        }
        PlayerStatManager.instance.AddAtkRangeModifire(playerNetworkType, increaseAmount);
    }
}

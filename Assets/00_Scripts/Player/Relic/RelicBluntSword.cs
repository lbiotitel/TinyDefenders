using UnityEngine;

public class RelicBluntSword : RelicBase
{
    [SerializeField] private int increaseAmount;

    public override void ActiveRelic()
    {
        if (playerNetworkType == PlayerNetworkType.CLIENT)
        { 
            NetworkRpcManager.instance.AddPlayerAtkModifireServerRpc((int)playerNetworkType, increaseAmount);
        }
        PlayerStatManager.instance.AddAtkModifire(playerNetworkType, increaseAmount);
    }
}

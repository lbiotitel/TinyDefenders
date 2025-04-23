using UnityEngine;

public class RelicBluntBow : RelicBase
{
    [SerializeField] private int increaseAmount;
    public override void ActiveRelic()
    {
        if (playerNetworkType == PlayerNetworkType.CLIENT)
        {
            NetworkRpcManager.instance.AddPlayerAtkSpeedModifireServerRpc((int)playerNetworkType, increaseAmount);
        }
        PlayerStatManager.instance.AddAtkSpeedModifire(playerNetworkType, increaseAmount);
    }
}

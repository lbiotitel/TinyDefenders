using UnityEngine;

public class RelicMaskOfRage : RelicBase
{
    [SerializeField] private int increaseAmount;
    public override void ActiveRelic()
    {
        if (playerNetworkType == PlayerNetworkType.CLIENT)
        {
            NetworkRpcManager.instance.AddCriDamageModifireServerRpc((int)playerNetworkType, increaseAmount);
        }

        PlayerStatManager.instance.AddCriDamageModifire(playerNetworkType, increaseAmount);
    }
}

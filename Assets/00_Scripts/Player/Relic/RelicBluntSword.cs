using UnityEngine;

public class RelicBluntSword : RelicBase
{
    [SerializeField] private int increaseAmount;

    public override void ActiveRelic()
    {
        Debug.Log("»£√‚1");
        PlayerStatManager.instance.AddAtkModifireServerRpc((int)playerNetworkType, increaseAmount);
    }
}

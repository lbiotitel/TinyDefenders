using UnityEngine;

public abstract class RelicBase : MonoBehaviour
{
    protected PlayerNetworkType playerNetworkType;

    public void InitPlayerNetworkType(PlayerNetworkType playerNetworkType)
    {
        this.playerNetworkType = playerNetworkType;
    }

    protected void OnValidate()
    {
        name = GetType().Name;
    }

    public abstract void ActiveRelic();
}

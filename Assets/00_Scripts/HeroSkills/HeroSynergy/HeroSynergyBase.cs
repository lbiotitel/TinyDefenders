using UnityEngine;


public abstract class HeroSynergyBase : MonoBehaviour
{
    protected PlayerNetworkType playerNetworkType;

    private void OnValidate()
    {
        name = GetType().Name;
    }

    public void InitNetworkType(PlayerNetworkType playerNetworkType)
    { 
        this.playerNetworkType = playerNetworkType;
    }

    public abstract void HeroSynergyActive(int level);
    public abstract void HeroSynergyDeactive(int level);
}

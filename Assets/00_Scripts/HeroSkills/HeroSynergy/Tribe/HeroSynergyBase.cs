using UnityEngine;


public abstract class HeroSynergyBase : MonoBehaviour
{
    protected PlayerNetworkType synergyNetworkType;
    protected string synergyName;
    protected int currentSynergyLevel;
    [SerializeField] private GameObject synergyParticle;

    private void OnValidate()
    {
        name = GetType().Name;
    }

    public void InitNetworkType(PlayerNetworkType playerNetworkType,string synergyName)
    { 
        this.synergyNetworkType = playerNetworkType;
        this.synergyName = synergyName;
    }

    protected bool CheckSynergyLevel(int level)
    {
        if (level < 1) return false;

        currentSynergyLevel = level;
        return true;
    }

    public abstract void HeroSynergyActive(int level);
    public abstract void HeroSynergyDeactive(int level);
    public GameObject GetSynergyParticle() { return synergyParticle; }
}

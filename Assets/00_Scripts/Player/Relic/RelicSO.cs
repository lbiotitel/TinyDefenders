using UnityEngine;
using UnityEngine.Experimental.AI;

[CreateAssetMenu(fileName = "RelicSO", menuName = "Scriptable Objects/RelicSO")]
public class RelicSO : ScriptableObject
{
    public string relicName;
    [TextArea] public string relicDesc;
    public Sprite relicImage;
    public int requireLevel;
    public int requirePoint;
    public RelicRarity relicRarity = RelicRarity.Normal;
    [SerializeField] private RelicBase relicBasePrefab;

    private bool relicUnlock = false;
    private bool relicOn = false;


    public void InitRelicSO(PlayerNetworkType playerNetworkType)
    { 
        relicBasePrefab = Instantiate(relicBasePrefab);
        relicBasePrefab.InitPlayerNetworkType(playerNetworkType);
        relicBasePrefab.ActiveRelic();
    }

    private void OnValidate()
    {
        relicName = name;
    }

    public bool IsEnoughLevel(int userLevel)
    {
        return userLevel >= requireLevel;
    }

    public bool IsEnoughPoint(int relicPoint)
    {
        return relicPoint >= requirePoint;
    }

    public void RelicOn()
    {
        relicOn = true;
    }
    public void RelicOff()
    {
        relicOn = false;
    }

    public bool IsRelicOn()
    {
        return relicOn;
    }

    public void UnlockRelic()
    {
        relicUnlock = true;
    }

    public void LockRelic()
    {
        relicUnlock = false;
    }

    public bool IsUnlock()
    {
        return relicUnlock;
    }
}

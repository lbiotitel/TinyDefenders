using Unity.Services.Multiplayer;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroSynergySO", menuName = "Scriptable Objects/HeroSynergySO")]
public class HeroSynergySO : ScriptableObject
{
    public HeroTribe synergyTribe;
    public string synergyName;
    [TextArea] public string synergyDescription;

    public int currentTribeCount = 0;
    public int[] synergyRequireTribeCount;
    public HeroSynergyBase synergyBase;

    private int currentSynergyLevel = 0;
    private int prevSynergyLevel = 0;

    public void InitSynergy(PlayerNetworkType playerNetworkType)
    {
        synergyBase.InitNetworkType(playerNetworkType);
    }

    public int CheckSynergyLevel()
    {
        for (int i = 0; i < synergyRequireTribeCount.Length; i++)
        {
            if (currentTribeCount < synergyRequireTribeCount[i]) break;
            currentSynergyLevel = i + 1;
        }

        return currentSynergyLevel;
    }

    public void ActiveSynegy()
    {
        if (currentSynergyLevel != prevSynergyLevel)
        {
            synergyBase.HeroSynergyDeactive(prevSynergyLevel - 1);
            synergyBase.HeroSynergyActive(currentSynergyLevel - 1);

            prevSynergyLevel = currentSynergyLevel;
        }
    }

    public void IncreaseTribeCount() { currentTribeCount++; }
    public void DecreaseTribeCount() { currentTribeCount = Mathf.Max(--currentTribeCount,0); }
}

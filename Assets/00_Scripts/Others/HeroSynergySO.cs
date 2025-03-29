using UnityEngine;

[CreateAssetMenu(fileName = "HeroSynergySO", menuName = "Scriptable Objects/HeroSynergySO")]
public class HeroSynergySO : ScriptableObject
{
    public HeroTribe synergyTribe;
    public string synergyName;
    [TextArea] public string[] synergyDescription;

    public int[] synergyRequireTribeCount;
    public HeroSynergyBase synergyBase;

    private int currentTribeCount = 0;
    private int currentSynergyLevel = 0;
    private int prevSynergyLevel = 0;

    public void InitSynergy(PlayerNetworkType playerNetworkType)
    {
        HeroSynergyBase heroSynergyBase = Instantiate(synergyBase);
        synergyBase = heroSynergyBase;
        synergyBase.InitNetworkType(playerNetworkType);
    }

    public int CheckSynergyLevel()
    {
        for (int i = 0; i < synergyRequireTribeCount.Length; i++)
        {
            if (currentTribeCount < synergyRequireTribeCount[i]) break;
            currentSynergyLevel = i;
        }

        return currentSynergyLevel;
    }

    public void ActiveSynegy()
    {
        if (currentSynergyLevel != prevSynergyLevel)
        {
            synergyBase.HeroSynergyDeactive(prevSynergyLevel);
            synergyBase.HeroSynergyActive(currentSynergyLevel);

            prevSynergyLevel = currentSynergyLevel;
        }
    }

    public string GetCurrentSynergyDescription()
    {
        return synergyDescription[currentSynergyLevel - 1];
    }


    public void SetCurrentHeroSynergyLevel(int level) { currentSynergyLevel = level; }
    public int GetCurrentSynergyLevel() { return currentSynergyLevel; } 
    public void IncreaseTribeCount() { currentTribeCount++; }
    public void DecreaseTribeCount() { currentTribeCount = Mathf.Max(--currentTribeCount, 0); }
}

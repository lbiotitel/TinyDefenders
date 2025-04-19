using UnityEngine;

public abstract class HeroSynergySOBase : ScriptableObject
{
    public string synergyName;
    [TextArea] public string[] synergyDescription;

    public int[] synergyRequireCount;
    public HeroSynergyBase synergyBase;

    protected int currentHeroCount = 0;
    protected int currentSynergyLevel = 0;
    protected int prevSynergyLevel = 0;

    public void InitSynergy(PlayerNetworkType playerNetworkType)
    {
        HeroSynergyBase heroSynergyBase = Instantiate(synergyBase);
        synergyBase = heroSynergyBase;
        synergyBase.InitNetworkType(playerNetworkType);
    }

    public int CheckSynergyLevel()
    {
        for (int i = 0; i < synergyRequireCount.Length; i++)
        {
            if (currentHeroCount < synergyRequireCount[i]) break;
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


    public abstract Sprite GetSynergyImage();
    public abstract void IncreaseHeroCount(HeroScriptable heroData);
    public abstract void DecreaseHeroCount(HeroScriptable heroData);
    public void SetCurrentHeroSynergyLevel(int level) { currentSynergyLevel = level; }
    public int GetCurrentSynergyLevel() { return currentSynergyLevel; }
}

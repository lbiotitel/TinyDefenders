using UnityEngine;

public abstract class HeroSynergySOBase : ScriptableObject
{
    public string synergyName;

    public int[] synergyRequireCount;
    public HeroSynergyBase synergyBase;

    protected int currentHeroCount = 0;
    protected int currentSynergyLevel = 0;
    protected int prevSynergyLevel = 0;

    public void InitSynergy(PlayerNetworkType playerNetworkType)
    {
        HeroSynergyBase heroSynergyBase = Instantiate(synergyBase);
        synergyBase = heroSynergyBase;
        synergyBase.InitNetworkType(playerNetworkType,synergyName);
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


    public abstract Sprite GetSynergyImage();
    public int GetHeroCount() { return currentHeroCount; }
    public abstract void IncreaseHeroCount(HeroScriptable heroData);
    public abstract void DecreaseHeroCount(HeroScriptable heroData);
    public void SetCurrentHeroSynergyLevel(int level) => currentSynergyLevel = level;
    public void SetCurrentHeroCount(int count) => currentHeroCount = count;
    public int GetCurrentSynergyLevel() { return currentSynergyLevel; }
    public string GetSynergyName() { return Utils.Localization_Text(Localize.Synergy,synergyName); }
    public string GetSynergyDescription() { return Utils.Localization_Text(Localize.Synergy, synergyName + " Desc"); }
}

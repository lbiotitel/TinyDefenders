using System.Collections.Generic;

[System.Serializable]
public class UserData
{
    [System.Serializable]
    public class HeroStoreData
    {
        public int heroLevel;
        public int heroCount;

        public HeroStoreData(int heroLevel, int heroCount)
        {
            this.heroLevel = heroLevel;
            this.heroCount = heroCount;
        }
    }

    [System.Serializable]
    public class SpellCardStoreData
    {
        public bool isOwned;

        public SpellCardStoreData(bool isOwned)
        {
            this.isOwned = isOwned;
        }
    }

    public string userName;
    public int userLevel;
    public int crystal;
    public int gold;
    public int rankingPoint;
    public Dictionary<string, HeroStoreData> heroDatas;
    public Dictionary<string, SpellCardStoreData> spellCardDatas;

    public UserData(string userName, int userLevel, int crystal, int gold, int rakingPoint)
    {
        this.userName = userName;
        this.userLevel = userLevel;
        this.crystal = crystal;
        this.gold = gold;
        this.rankingPoint = rakingPoint;
        this.heroDatas = new Dictionary<string, HeroStoreData>();
        this.spellCardDatas = new Dictionary<string, SpellCardStoreData>();

        InitHeroDatas();
        InitSpellCardDatas();
    }

    private void InitHeroDatas()
    {
        List<HeroScriptable> heroDataList = SoDataManager.instance.GetAllHeroScriptableData();

        for (int i = 0; i < heroDataList.Count; i++)
        {
            heroDatas.Add(heroDataList[i].heroName, new HeroStoreData(0, 0));
        }
    }

    private void InitSpellCardDatas()
    {
        List<CardSO> spellCardList = SoDataManager.instance.GetAllCardScriptableObjectData();

        for (int i = 0; i < spellCardList.Count; i++)
        {
            spellCardDatas.Add(spellCardList[i].cardName, new SpellCardStoreData(false));
        }
    }
}
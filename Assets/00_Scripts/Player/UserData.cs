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
    public int rankingPoint;
    public int userExp;
    public int totalPlayCount;
    public int stopPlayCount;
    public int relicPoint;
    public Dictionary<string, HeroStoreData> heroDataDic;
    public Dictionary<string, SpellCardStoreData> spellCardDataDic;
    public Dictionary<string, bool> relicDataDic;

    public UserData(string userName, int userLevel, int rakingPoint)
    {
        this.userName = userName;
        this.userLevel = userLevel;
        this.rankingPoint = rakingPoint;
        this.userExp = 0;
        this.totalPlayCount = 0;
        this.stopPlayCount = 0;
        this.relicPoint = 0;

        this.heroDataDic = new Dictionary<string, HeroStoreData>();
        this.spellCardDataDic = new Dictionary<string, SpellCardStoreData>();
        this.relicDataDic = new Dictionary<string, bool>();

        InitHeroDatas();
        InitSpellCardDatas();
        InitRelicDatas();
    }

    private void InitHeroDatas()
    {
        List<HeroScriptable> heroDataList = GameResourceManager.instance.GetAllHeroScriptableData();

        if (heroDataList == null) return;

        for (int i = 0; i < heroDataList.Count; i++)
        {
            heroDataDic.Add(heroDataList[i].heroName, new HeroStoreData(0, 0));
        }
    }

    private void InitSpellCardDatas()
    {
        List<CardSO> spellCardList = GameResourceManager.instance.GetAllCardScriptableObjectData();

        for (int i = 0; i < spellCardList.Count; i++)
        {
            spellCardDataDic.Add(spellCardList[i].cardName, new SpellCardStoreData(false));
        }
    }


    private void InitRelicDatas()
    {
        List<RelicSO> relicDataList = GameResourceManager.instance.GetAllRelicData();

        for (int i = 0; i < relicDataList.Count; i++)
        {
            relicDataDic.Add(relicDataList[i].relicName, relicDataList[i].IsRelicOn());
        }
    }
}
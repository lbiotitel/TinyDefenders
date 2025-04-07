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
    public int userExp;
    public int totalPlayCount;
    public int stopPlayCount;
    public int remainSkillPoint;
    public Dictionary<string, HeroStoreData> heroDataDic;
    public Dictionary<string, SpellCardStoreData> spellCardDataDic;
    public Dictionary<PlayerSkillType, int> playerSkillDataDic;

    public UserData(string userName, int userLevel, int crystal, int gold, int rakingPoint)
    {
        this.userName = userName;
        this.userLevel = userLevel;
        this.crystal = crystal;
        this.gold = gold;
        this.rankingPoint = rakingPoint;
        this.heroDataDic = new Dictionary<string, HeroStoreData>();
        this.spellCardDataDic = new Dictionary<string, SpellCardStoreData>();
        this.userExp = 0;
        this.totalPlayCount = 0;
        this.stopPlayCount = 0;
        this.remainSkillPoint = 0;

        InitHeroDatas();
        InitSpellCardDatas();
        InitPlayerSkillDatas();
    }

    private void InitHeroDatas()
    {
        List<HeroScriptable> heroDataList = SoDataManager.instance.GetAllHeroScriptableData();

        if (heroDataList == null) return;

        for (int i = 0; i < heroDataList.Count; i++)
        {
            heroDataDic.Add(heroDataList[i].heroName, new HeroStoreData(0, 0));
        }
    }

    private void InitSpellCardDatas()
    {
        List<CardSO> spellCardList = SoDataManager.instance.GetAllCardScriptableObjectData();

        for (int i = 0; i < spellCardList.Count; i++)
        {
            spellCardDataDic.Add(spellCardList[i].cardName, new SpellCardStoreData(false));
        }
    }


    private void InitPlayerSkillDatas()
    {
        List<PlayerSkillSO> playerSkillList = SoDataManager.instance.GetAllPlayerSkillData();

        for (int i = 0; i < playerSkillList.Count; i++)
        {
            playerSkillDataDic.Add(playerSkillList[i].playerSkillType, 0);
        }
    }
}
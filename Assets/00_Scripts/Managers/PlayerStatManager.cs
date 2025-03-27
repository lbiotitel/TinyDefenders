using System.Collections.Generic;
using UnityEngine;


public class HeroSynergyUI {
    public string synergyName;
    public string synergyDescription;
    public int synergyCurrentLevel;
}

public class PlayerStatManager : MonoBehaviour
{
    public static PlayerStatManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    [Header("Hero Upgrade")]
    [field: SerializeField] public InGameHeroUpgrade clientUpgrade { get; private set; }
    [field: SerializeField] public InGameHeroUpgrade serverUpgrade { get; private set; }

    private Dictionary<HeroTribe, HeroSynergySO> clientHeroSynergyDic = new Dictionary<HeroTribe, HeroSynergySO>();
    private Dictionary<HeroTribe, HeroSynergySO> serverHeroSynergyDic = new Dictionary<HeroTribe, HeroSynergySO>();

    private void Start()
    {
        InitHeroSynergy();
    }



    #region Hero Synergy
    private void InitHeroSynergy()
    {
        HeroSynergySO[] clientHeroSynergySOs = Resources.LoadAll<HeroSynergySO>("HeroSynergyDatas");


        foreach (var heroSynergy in clientHeroSynergySOs)
        {
            clientHeroSynergyDic.Add(heroSynergy.synergyTribe, heroSynergy);
            HeroSynergySO heroSynergySO = Instantiate(heroSynergy);
            serverHeroSynergyDic.Add(heroSynergySO.synergyTribe, heroSynergySO);
        }
    }

    public void CheckHeroSynergy(Hero hero)
    {
        if (NetUtils.IsServerHolder(hero))
        {
            HeroSynergySO heroSynergySO = serverHeroSynergyDic[hero.GetHeroData().heroTribe];
            heroSynergySO.currentTribeCount++;
        }
        else
        {
            HeroSynergySO heroSynergySO = clientHeroSynergyDic[hero.GetHeroData().heroTribe];
            heroSynergySO.currentTribeCount++;
        }

        CheckSynergy();
    }

    private void CheckSynergy()
    {
        foreach (var data in serverHeroSynergyDic)
        {

        }

        foreach (var data in clientHeroSynergyDic)
        {

        }
    }


    public List<HeroSynergyUI> GetHeroSynergyUI()
    {
        List<HeroSynergyUI> heroSynergyUIs = new List<HeroSynergyUI>();

        foreach (var data in serverHeroSynergyDic)
        {
            HeroSynergyUI heroSynergyUI = new HeroSynergyUI();
            heroSynergyUI.synergyName = data.Value.synergyName;
            heroSynergyUI.synergyDescription = data.Value.synergyDescription;
            heroSynergyUI.synergyCurrentLevel = data.Value.CheckSynergyLevel();
        }

        return heroSynergyUIs;
    }


    #endregion


    #region Hero Upgrade


    public int GetHeroUpgradeLevel(HeroTribe heroTribe)
    {
        return NetUtils.IsServer() == true ?
            serverUpgrade.GetHeroUpgradeLevel(heroTribe) : clientUpgrade.GetHeroUpgradeLevel(heroTribe);
    }

    public int GetHeroUpgradeLevel(HeroTribe heroTribe, bool isServerHero)
    {
        return isServerHero == true ?
            serverUpgrade.GetHeroUpgradeLevel(heroTribe) : clientUpgrade.GetHeroUpgradeLevel(heroTribe);
    }

    public int GetHeroUpgradeCost(HeroTribe heroTribe)
    {
        return NetUtils.IsServer() == true ?
            serverUpgrade.GetHeroUpgradeCost(heroTribe) : clientUpgrade.GetHeroUpgradeCost(heroTribe);
    }
    public int GetHeroUpgradeCost(HeroTribe heroTribe, bool isServerHero)
    {
        return isServerHero == true ?
            serverUpgrade.GetHeroUpgradeCost(heroTribe) : clientUpgrade.GetHeroUpgradeCost(heroTribe);
    }

    #endregion

}

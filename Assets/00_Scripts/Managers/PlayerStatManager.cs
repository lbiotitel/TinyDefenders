using System.Collections.Generic;
using UnityEngine;


public class HeroSynergyUI
{
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


    [Header("Server Bonus Stat")]
    [field: SerializeField] public Stat serverAtk { get; private set; }
    [field: SerializeField] public Stat serverAtkSpeed { get; private set; }
    [field: SerializeField] public Stat serverGoldBonus { get; private set; }



    [Header("Client Bonus Stat")]
    [field: SerializeField] public Stat clientAtk { get; private set; }
    [field: SerializeField] public Stat clientAtkSpeed { get; private set; }
    [field: SerializeField] public Stat clientGoldBouns { get; private set; }



    private Dictionary<HeroTribe, HeroSynergySO> clientHeroSynergyDic = new Dictionary<HeroTribe, HeroSynergySO>();
    private Dictionary<HeroTribe, HeroSynergySO> serverHeroSynergyDic = new Dictionary<HeroTribe, HeroSynergySO>();

    private void Start()
    {
        InitHeroSynergy();
    }



    #region Hero Synergy
    private void InitHeroSynergy()
    {
        HeroSynergySO[] heroSynergySOs = Resources.LoadAll<HeroSynergySO>("HeroSynergyDatas");


        foreach (var heroSynergy in heroSynergySOs)
        {
            HeroSynergySO clientSynergySos = Instantiate(heroSynergy);
            clientSynergySos.InitSynergy(PlayerNetworkType.CLIENT);
            clientHeroSynergyDic.Add(clientSynergySos.synergyTribe, clientSynergySos);

            HeroSynergySO serverHeroSynergySOs = Instantiate(heroSynergy);
            serverHeroSynergySOs.InitSynergy(PlayerNetworkType.HOST);
            serverHeroSynergyDic.Add(serverHeroSynergySOs.synergyTribe, serverHeroSynergySOs);
        }
    }

    public void AddHero(Hero hero)
    {
        if (hero.heroOwner == NetworkObjectType.HOST)
        {
            Debug.Log("서버 영웅 추가");
            serverHeroSynergyDic[hero.GetHeroData().heroTribe]?.IncreaseTribeCount();
        }
        else
        {
            Debug.Log("클라 영웅 추가");
            clientHeroSynergyDic[hero.GetHeroData().heroTribe]?.IncreaseTribeCount();
        }

        CheckSynergy();
    }

    public void RemoveHero(Hero hero)
    {
        if (hero.heroOwner == NetworkObjectType.HOST)
        {
            serverHeroSynergyDic[hero.GetHeroData().heroTribe]?.DecreaseTribeCount();
        }
        else
        {
            clientHeroSynergyDic[hero.GetHeroData().heroTribe]?.DecreaseTribeCount();
        }

        CheckSynergy();
    }

    private void CheckSynergy()
    {
        foreach (var data in serverHeroSynergyDic)
        {
            if (data.Value.CheckSynergyLevel() > 0)
            {
                data.Value.ActiveSynegy();
            }
        }

        foreach (var data in clientHeroSynergyDic)
        {
            if (data.Value.CheckSynergyLevel() > 0)
            {
                data.Value.ActiveSynegy();
            }
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

    #region About Player Stat

    public void AddAtkModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverAtk.AddModifire(value);
                break;
            case PlayerNetworkType.CLIENT:
                clientAtk.AddModifire(value);
                break;
        }
    }

    public void RemoveAtkModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverAtk.RemoveModifire(value);
                break;
            case PlayerNetworkType.CLIENT:
                clientAtk.RemoveModifire(value);
                break;
        }
    }

    public void AddAtkSpeedModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverAtkSpeed.AddModifire(value);
                break;
            case PlayerNetworkType.CLIENT:
                clientAtkSpeed.AddModifire(value);
                break;
        }
    }
    public void RemoveAtkSpeedModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverAtkSpeed.RemoveModifire(value);
                break;
            case PlayerNetworkType.CLIENT:
                clientAtkSpeed.RemoveModifire(value);
                break;
        }
    }


    #endregion


    #region Hero Upgrade


    public int GetHeroUpgradeLevel(HeroTribe heroTribe)
    {
        return NetUtils.IsServer() == true ?
            serverUpgrade.GetHeroUpgradeLevel(heroTribe) : clientUpgrade.GetHeroUpgradeLevel(heroTribe);
    }

    public int GetHeroUpgradeLevel(HeroTribe heroTribe, NetworkObjectType heroOwner)
    {
        return heroOwner == NetworkObjectType.HOST ?
            serverUpgrade.GetHeroUpgradeLevel(heroTribe) : clientUpgrade.GetHeroUpgradeLevel(heroTribe);
    }

    public int GetHeroUpgradeCost(HeroTribe heroTribe)
    {
        return NetUtils.IsServer() == true ?
            serverUpgrade.GetHeroUpgradeCost(heroTribe) : clientUpgrade.GetHeroUpgradeCost(heroTribe);
    }
    public int GetHeroUpgradeCost(HeroTribe heroTribe, NetworkObjectType heroOwner)
    {
        return heroOwner == NetworkObjectType.HOST ?
            serverUpgrade.GetHeroUpgradeCost(heroTribe) : clientUpgrade.GetHeroUpgradeCost(heroTribe);
    }

    #endregion

}

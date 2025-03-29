using System.Collections.Generic;
using Unity.Services.Multiplayer;
using UnityEngine;



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
    [field: SerializeField] public Stat serverAtk;
    [field: SerializeField] public Stat serverAtkSpeed;
    [field: SerializeField] public Stat serverGoldBonus;
    [field: SerializeField] public Stat serverSkillAcc;



    [Header("Client Bonus Stat")]
    [field: SerializeField] public Stat clientAtk;
    [field: SerializeField] public Stat clientAtkSpeed;
    [field: SerializeField] public Stat clientGoldBouns;
    [field: SerializeField] public Stat clientSkillAcc;



    private Dictionary<HeroTribe, HeroSynergySO> clientHeroSynergyDic = new Dictionary<HeroTribe, HeroSynergySO>();
    private Dictionary<HeroTribe, HeroSynergySO> serverHeroSynergyDic = new Dictionary<HeroTribe, HeroSynergySO>();

    private void Start()
    {
        InitHeroSynergy();
    }



    #region Hero Synergy
    private void InitHeroSynergy()
    {

        List<HeroSynergySO> heroSynergySOs = SoDataManager.instance.GetHeroSynergySOs();

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
        
        SendSynergyDataToClient();
    }

    private void SendSynergyDataToClient()
    {
        if (NetUtils.IsHost())
        {
            int[] heroTribe = new int[(int)HeroTribe.LAST];
            int[] heroSynergyLevel = new int[(int)HeroTribe.LAST];

            int index = 0;
            foreach (var data in clientHeroSynergyDic)
            {
                heroTribe[index] = (int)data.Key;
                heroSynergyLevel[index] = data.Value.GetCurrentSynergyLevel();
                index++;
            }

            NetworkRpcManager.instance.GetHHeroSynergyDatasClientRpc(heroTribe, heroSynergyLevel);
        }
    }


    public void SetClientHeroSynergyData(PlayerNetworkType playerNetworkType, HeroTribe heroTribe, int synergyLevel)
    {
        if (playerNetworkType == PlayerNetworkType.CLIENT)
        {
            clientHeroSynergyDic[heroTribe].SetCurrentHeroSynergyLevel(synergyLevel);
            Debug.Log(heroTribe.ToString() + synergyLevel.ToString());
        }
        EventManager.instance.onHeroSynergyChangedAction?.Invoke();
    }

    public List<HeroSynergySO> GetHeroSynergyData(PlayerNetworkType playerNetworkType)
    {
        List<HeroSynergySO> heroSynergySOs = new List<HeroSynergySO>();

        Dictionary<HeroTribe, HeroSynergySO> referDic = null;
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                referDic = serverHeroSynergyDic;
                break;
            case PlayerNetworkType.CLIENT:
                referDic = clientHeroSynergyDic;

                break;
        }

        foreach (var data in referDic)
        {
            heroSynergySOs.Add(data.Value);
        }

        return heroSynergySOs;
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

    public void AddSkillAccModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverSkillAcc.AddModifire(value);
                break;
            case PlayerNetworkType.CLIENT:
                clientSkillAcc.AddModifire(value);
                break;
        }
    }

    public void RemoveSkillAccModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverSkillAcc.RemoveModifire(value);
                break;
            case PlayerNetworkType.CLIENT:
                clientSkillAcc.RemoveModifire(value);
                break;
        }
    }

    public double GetBonusAtk(PlayerNetworkType playerNetworkType)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                return serverAtk.GetValue();
            case PlayerNetworkType.CLIENT:
                return clientAtk.GetValue();
        }

        return 0;
    }


    public double GetBonusAtk(NetworkObjectType heroNetworkType)
    {
        switch (heroNetworkType)
        {
            case NetworkObjectType.HOST:
                return serverAtk.GetValue();
            case NetworkObjectType.CLIENT:
                return clientAtk.GetValue();
        }

        return 0;
    }

    public float GetBonusAtkSpeed(PlayerNetworkType playerNetworkType)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                return serverAtkSpeed.GetValue();
            case PlayerNetworkType.CLIENT:
                return clientAtkSpeed.GetValue();
        }

        return 0;
    }


    public float GetBonusAtkSpeed(NetworkObjectType heroNetworkType)
    {
        switch (heroNetworkType)
        {
            case NetworkObjectType.HOST:
                return serverAtkSpeed.GetValue();
            case NetworkObjectType.CLIENT:
                return clientAtkSpeed.GetValue();
        }

        return 0;
    }

    public float GetBonusSkillAcc(PlayerNetworkType playerNetworkType)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                return serverSkillAcc.GetValue();
            case PlayerNetworkType.CLIENT:
                return clientSkillAcc.GetValue();
        }

        return 0;
    }

    public float GetBonusSkillAcc(NetworkObjectType heroNetworkType)
    {
        switch (heroNetworkType)
        {
            case NetworkObjectType.HOST:
                return serverSkillAcc.GetModifireValue();
            case NetworkObjectType.CLIENT:
                return clientSkillAcc.GetModifireValue();
        }

        return 0;
    }


    #endregion


    #region Hero Upgrade


    public int GetHeroUpgradeLevel(HeroTribe heroTribe)
    {
        return NetUtils.IsHost() == true ?
            serverUpgrade.GetHeroUpgradeLevel(heroTribe) : clientUpgrade.GetHeroUpgradeLevel(heroTribe);
    }

    public int GetHeroUpgradeLevel(HeroTribe heroTribe, NetworkObjectType heroOwner)
    {
        return heroOwner == NetworkObjectType.HOST ?
            serverUpgrade.GetHeroUpgradeLevel(heroTribe) : clientUpgrade.GetHeroUpgradeLevel(heroTribe);
    }

    public int GetHeroUpgradeCost(HeroTribe heroTribe)
    {
        return NetUtils.IsHost() == true ?
            serverUpgrade.GetHeroUpgradeCost(heroTribe) : clientUpgrade.GetHeroUpgradeCost(heroTribe);
    }
    public int GetHeroUpgradeCost(HeroTribe heroTribe, NetworkObjectType heroOwner)
    {
        return heroOwner == NetworkObjectType.HOST ?
            serverUpgrade.GetHeroUpgradeCost(heroTribe) : clientUpgrade.GetHeroUpgradeCost(heroTribe);
    }

    #endregion

}

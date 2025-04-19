using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
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

    [Header("Player Stat")]
    [field: SerializeField] public int ingameLevel { get; private set; } = 1;
    [field: SerializeField] public int ingameExp { get; private set; } = 0;
    [field: SerializeField] public int currentMaxHeroCount { get; private set; } = 3;
    [SerializeField] public int maxHeroCount { get; set; }
    [SerializeField] private int giveExpAmount;
    [SerializeField] private int expGold;

    [Header("Hero Upgrade")]
    [field: SerializeField] public InGameUpgrade clientUpgrade { get; private set; }
    [field: SerializeField] public InGameUpgrade serverUpgrade { get; private set; }


    [Header("Server Bonus Stat")]
    [field: SerializeField] public Stat serverAtk;
    [field: SerializeField] public Stat serverAtkSpeed;
    [field: SerializeField] public Stat serverAtkRange;
    [field: SerializeField] public Stat serverGoldBonus;
    [field: SerializeField] public Stat serverSkillAcc;
    [field: SerializeField] public Stat serverCriticalRate;
    [field: SerializeField] public Stat serverCriDamage;
    [field: SerializeField] public Stat serverDebuffDuration;



    [Header("Client Bonus Stat")]
    [field: SerializeField] public Stat clientAtk;
    [field: SerializeField] public Stat clientAtkSpeed;
    [field: SerializeField] public Stat clientAtkRange;
    [field: SerializeField] public Stat clientGoldBouns;
    [field: SerializeField] public Stat clientSkillAcc;
    [field: SerializeField] public Stat clientCriticalRate;
    [field: SerializeField] public Stat clientCriDamage;
    [field: SerializeField] public Stat clientDebuffDuration;

    private Dictionary<string, HeroSynergySOBase> clientHeroSynergyDic = new Dictionary<string, HeroSynergySOBase>();
    private Dictionary<string, HeroSynergySOBase> serverHeroSynergyDic = new Dictionary<string, HeroSynergySOBase>();

    private void Start()
    {
        InitHeroSynergy();
        if (NetUtils.IsHost())
        {
            EventManager.instance.onHeroSummonAction += AddHero;
            EventManager.instance.onHeroDespawnAction += RemoveHero;
        }
    }


    #region

    public void AddIngameExp()
    {
        if (GameManager.instance.IsEnoughGold(expGold))
        {
            GameManager.instance.SpendGold(expGold);
            ingameExp += giveExpAmount;
            CheckLevelUp();
        }
    }

    private void CheckLevelUp()
    {
        int requireExp = GameResourceManager.instance.GetRequireExp(ingameLevel);
        if (requireExp <= ingameExp)
        { 
            ingameExp -= requireExp;
            LevelUp();
        }
        EventManager.instance.onExpLevelChangedAction?.Invoke(ingameExp, GameResourceManager.instance.GetRequireExp(ingameLevel));
    }

    private void LevelUp()
    {
        ingameLevel++;
        currentMaxHeroCount = Mathf.Min(++currentMaxHeroCount,maxHeroCount);
        EventManager.instance.onLevelChagendAction?.Invoke(ingameLevel);
    }

    #endregion


    #region Hero Synergy
    private void InitHeroSynergy()
    {

        List<HeroSynergySOBase> heroSynergySOs = GameResourceManager.instance.GetHeroSynergySOs();

        foreach (var heroSynergy in heroSynergySOs)
        {
            HeroSynergySOBase clientSynergySos = Instantiate(heroSynergy);
            clientSynergySos.InitSynergy(PlayerNetworkType.CLIENT);
            clientHeroSynergyDic.Add(clientSynergySos.synergyName, clientSynergySos);

            HeroSynergySOBase serverHeroSynergySOs = Instantiate(heroSynergy);
            serverHeroSynergySOs.InitSynergy(PlayerNetworkType.HOST);
            serverHeroSynergyDic.Add(serverHeroSynergySOs.synergyName, serverHeroSynergySOs);
        }
    }

    public void AddHero(Hero hero)
    {
        if (GameManager.instance.isGameOver) return;

        HeroScriptable heroData = hero.GetHeroData();

        if (hero.heroOwnerNetworkType == PlayerNetworkType.HOST)
        {
            foreach (var synergy in serverHeroSynergyDic)
            { 
                synergy.Value.IncreaseHeroCount(heroData);
            }
        }
        else
        {
            foreach (var synergy in clientHeroSynergyDic)
            {
                synergy.Value.IncreaseHeroCount(heroData);
            }
        }

        if(NetUtils.IsHost())
            CheckSynergy();
    }

    public void RemoveHero(Hero hero)
    {
        if (GameManager.instance.isGameOver) return;

        HeroScriptable heroData = hero.GetHeroData();

        if (hero.heroOwnerNetworkType == PlayerNetworkType.HOST)
        {
            foreach (var synergy in serverHeroSynergyDic)
            {
                synergy.Value.DecreaseHeroCount(heroData);
            }
        }
        else
        {
            foreach (var synergy in clientHeroSynergyDic)
            {
                synergy.Value.DecreaseHeroCount(heroData);
            }
        }

        if(NetUtils.IsHost())
            CheckSynergy();
    }

    private void CheckSynergy()
    {
        foreach (var data in serverHeroSynergyDic)
        {
            data.Value.CheckSynergyLevel();
            data.Value.ActiveSynegy();
        }

        foreach (var data in clientHeroSynergyDic)
        {
            data.Value.CheckSynergyLevel();
            data.Value.ActiveSynegy();
        }
        
        SendSynergyDataToClient();
    }

    private void SendSynergyDataToClient()
    {
        if (NetUtils.IsHost())
        {
            FixedString32Bytes[] synergyName = new FixedString32Bytes[clientHeroSynergyDic.Count];
            int[] heroSynergyLevel = new int[(int)HeroTribe.LAST];

            int index = 0;
            foreach (var data in clientHeroSynergyDic)
            {
                synergyName[index] = data.Key;
                heroSynergyLevel[index] = data.Value.GetCurrentSynergyLevel();
                index++;
            }

            NetworkRpcManager.instance.GetHeroSynergyDatasClientRpc(synergyName, heroSynergyLevel);
        }
    }


    public void SetClientHeroSynergyData(PlayerNetworkType playerNetworkType, string synergyName, int synergyLevel)
    {
        if (playerNetworkType == PlayerNetworkType.CLIENT)
        {
            clientHeroSynergyDic[synergyName].SetCurrentHeroSynergyLevel(synergyLevel);

            foreach (var data in clientHeroSynergyDic)
            {
                data.Value.ActiveSynegy();
            }
        }
        EventManager.instance.onHeroSynergyChangedAction?.Invoke();
    }

    public List<HeroSynergySOBase> GetHeroSynergyData(PlayerNetworkType playerNetworkType)
    {
        List<HeroSynergySOBase> heroSynergySOs = new List<HeroSynergySOBase>();

        Dictionary<string, HeroSynergySOBase> referDic = null;

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

    #region About Player Stat Modifier
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

    public void AddCriticalRateModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverCriticalRate.AddModifire(value);
                break;
            case PlayerNetworkType.CLIENT:
                clientCriticalRate.AddModifire(value);
                break;
        }
    }

    public void RemoveCriticalRateModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverCriticalRate.RemoveModifire(value);
                break;
            case PlayerNetworkType.CLIENT:
                clientCriticalRate.RemoveModifire(value);
                break;
        }
    }

    public void AddCriDamageModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverCriDamage.AddModifire(value);
                break;
            case PlayerNetworkType.CLIENT:
                clientCriDamage.AddModifire(value);
                break;
        }
    }

    public void RemoveCriDamageModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverCriDamage.RemoveModifire(value);
                break;
            case PlayerNetworkType.CLIENT:
                clientCriDamage.RemoveModifire(value);
                break;
        }
    }


    public void AddAtkRangeModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverAtkRange.AddModifire(value);
                break;
            case PlayerNetworkType.CLIENT:
                clientAtkRange.AddModifire(value);
                break;
        }
    }

    public void RemoveAtkRangeModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverAtkRange.RemoveModifire(value);
                break;
            case PlayerNetworkType.CLIENT:
                clientAtkRange.RemoveModifire(value);
                break;
        }
    }

    public void AddGoudBonusModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverGoldBonus.AddModifire(value);
                break;
            case PlayerNetworkType.CLIENT:
                clientGoldBouns.AddModifire(value);
                break;
        }
    }

    public void RemoveGoldBonusModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverGoldBonus.RemoveModifire(value);
                break;
            case PlayerNetworkType.CLIENT:
                clientGoldBouns.RemoveModifire(value);
                break;
        }
    }

    #endregion

    #region About Get Player Stat

    public double GetBonusAtk(PlayerNetworkType playerNetworkType)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                return serverAtk.GetModifireValue();
            case PlayerNetworkType.CLIENT:
                return clientAtk.GetModifireValue();
        }

        return 0;
    }


    public double GetBonusAtk(NetworkObjectType heroNetworkType)
    {
        switch (heroNetworkType)
        {
            case NetworkObjectType.HOST:
                return serverAtk.GetModifireValue();
            case NetworkObjectType.CLIENT:
                return clientAtk.GetModifireValue();
        }

        return 0;
    }

    public float GetBonusAtkSpeed(PlayerNetworkType playerNetworkType)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                return serverAtkSpeed.GetModifireValue();
            case PlayerNetworkType.CLIENT:
                return clientAtkSpeed.GetModifireValue();
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

    public float GetBonusAtkRange(PlayerNetworkType playerNetworkType)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                return serverAtkRange.GetModifireValue();
            case PlayerNetworkType.CLIENT:
                return clientAtkRange.GetModifireValue();
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


    public float GetBonusCriticalRate(PlayerNetworkType playerNetworkType)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                return serverCriticalRate.GetModifireValue();
            case PlayerNetworkType.CLIENT:
                return clientCriticalRate.GetModifireValue();
        }

        return 0;
    }

    public float GetBonusCriDamage(PlayerNetworkType playerNetworkType)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                return serverCriDamage.GetModifireValue();
            case PlayerNetworkType.CLIENT:
                return clientCriDamage.GetModifireValue();
        }

        return 0;
    }

    public float GetBonusGold(PlayerNetworkType playerNetworkType)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                return serverGoldBonus.GetModifireValue();
            case PlayerNetworkType.CLIENT:
                return clientGoldBouns.GetModifireValue();
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

    public int GetHeroUpgradeLevel(HeroTribe heroTribe, PlayerNetworkType heroOwner)
    {
        return heroOwner == PlayerNetworkType.HOST ?
            serverUpgrade.GetHeroUpgradeLevel(heroTribe) : clientUpgrade.GetHeroUpgradeLevel(heroTribe);
    }

    public int GetHeroUpgradeCost(HeroTribe heroTribe)
    {
        return NetUtils.IsHost() == true ?
            serverUpgrade.GetHeroUpgradeCost(heroTribe) : clientUpgrade.GetHeroUpgradeCost(heroTribe);
    }
    public int GetHeroUpgradeCost(HeroTribe heroTribe, PlayerNetworkType heroOwner)
    {
        return heroOwner == PlayerNetworkType.HOST ?
            serverUpgrade.GetHeroUpgradeCost(heroTribe) : clientUpgrade.GetHeroUpgradeCost(heroTribe);
    }

    #endregion



}

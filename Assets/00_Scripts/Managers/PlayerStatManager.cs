using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;


public enum PlayerStatType 
{
    Atk,
    AtkSpeed,
    AtkRange,
    CriticalRate,
    CriticalDamage,
    SkillAcc,
    BuffBonus,
    BuffRangeBonus
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

    [Header("Player Stat")]
    [field: SerializeField] public int ingameLevel { get; private set; } = 1;
    [field: SerializeField] public int ingameExp { get; private set; } = 0;
    [field: SerializeField] public int currentMaxHeroCount { get; private set; } = 3;
    [field: SerializeField] public int maxHeroCount { get;  set; }
    [field: SerializeField] public int maxHeroLevel { get; private set; }
    [field: SerializeField] public int requireCombineHeroCount { get; set; }

    [SerializeField] private int giveExpAmount;
    [SerializeField] private int expGold;

    [Header("Server Bonus Stat")]
    [field: SerializeField] public Stat serverAtk;
    [field: SerializeField] public Stat serverAtkSpeed;
    [field: SerializeField] public Stat serverAtkRange;
    [field: SerializeField] public Stat serverGoldBonus;
    [field: SerializeField] public Stat serverSkillAcc;
    [field: SerializeField] public Stat serverCriticalRate;
    [field: SerializeField] public Stat serverCriDamage;
    [field: SerializeField] public Stat serverDebuffDuration;
    [field:SerializeField]  public Stat serverBuffBonus;
    [field: SerializeField] public float serverBuffRangeBounus;




    [Header("Client Bonus Stat")]
    [field: SerializeField] public Stat clientAtk;
    [field: SerializeField] public Stat clientAtkSpeed;
    [field: SerializeField] public Stat clientAtkRange;
    [field: SerializeField] public Stat clientGoldBouns;
    [field: SerializeField] public Stat clientSkillAcc;
    [field: SerializeField] public Stat clientCriticalRate;
    [field: SerializeField] public Stat clientCriDamage;
    [field: SerializeField] public Stat clientDebuffDuration;
    [field: SerializeField] public Stat clientBuffBonus; 
    [field: SerializeField] public float clientBuffRangeBounus;

    private Dictionary<string, HeroSynergySOBase> clientHeroSynergyDic = new Dictionary<string, HeroSynergySOBase>();
    private Dictionary<string, HeroSynergySOBase> serverHeroSynergyDic = new Dictionary<string, HeroSynergySOBase>();

    private void Start()
    {
        InitHeroSynergy();
        if (NetUtils.IsHost())
        {
            EventManager.instance.onHeroSpawnAction += AddHero;
            EventManager.instance.onHeroDespawnAction += RemoveHero;
        }

        EventManager.instance.onPlayerAddStatModifireAction += AddStatModifire;
        EventManager.instance.onPlayerRemoveStatModifireAction += RemoveStatModifire;
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

    public void AddIngameExp(int exp)
    {
        ingameExp += exp;
        CheckLevelUp();
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
        EventManager.instance.onMaxHeroCountChangedAction?.Invoke(currentMaxHeroCount);
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
            int[] heroCount = new int[serverHeroSynergyDic.Count];
            int[] heroSynergyLevel = new int[serverHeroSynergyDic.Count];

            int index = 0;
            foreach (var data in clientHeroSynergyDic)
            {
                synergyName[index] = data.Key;
                heroCount[index] = data.Value.GetHeroCount();
                heroSynergyLevel[index] = data.Value.GetCurrentSynergyLevel();
                index++;
            }

            NetworkRpcManager.instance.GetHeroSynergyDatasClientRpc(synergyName,heroCount, heroSynergyLevel);
        }
    }


    public void SetClientHeroSynergyData(PlayerNetworkType playerNetworkType, string synergyName,int heroCount, int synergyLevel)
    {
        if (playerNetworkType == PlayerNetworkType.CLIENT)
        {
            clientHeroSynergyDic[synergyName].SetCurrentHeroSynergyLevel(synergyLevel);
            clientHeroSynergyDic[synergyName].SetCurrentHeroCount(heroCount);
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

    public void AddStatModifire(PlayerNetworkType playerNetworkType, PlayerStatType statType, float value) => NetworkRpcManager.instance.AddPlayerStatModifireServerRpc((int)playerNetworkType, (int)statType, value);

    private void AddStatModifire(int networkType, int statType, float value)
    {
        PlayerStatType type = (PlayerStatType)statType;
        PlayerNetworkType playerNetworkType = (PlayerNetworkType)networkType;

        switch (type)
        {
            case PlayerStatType.Atk:
                AddAtkModifire(playerNetworkType, value);
                break;
            case PlayerStatType.AtkSpeed:
                AddAtkSpeedModifire(playerNetworkType, value);
                break;
            case PlayerStatType.AtkRange:
                AddAtkRangeModifire(playerNetworkType, value);
                break;
            case PlayerStatType.CriticalRate:
                AddCriticalRateModifire(playerNetworkType, value);
                break;
            case PlayerStatType.CriticalDamage:
                AddCriDamageModifire(playerNetworkType, value);
                break;
            case PlayerStatType.SkillAcc:
                AddSkillAccModifire(playerNetworkType, value);
                break;
            case PlayerStatType.BuffBonus:
                AddBuffBonusModifire(playerNetworkType,value);
                break;
            case PlayerStatType.BuffRangeBonus:
                AddBuffRangeBonusModifire(playerNetworkType,value);
                break;
        }
    }

    public void RemoveStatModifire(PlayerNetworkType playerNetworkType, PlayerStatType statType, float value) => NetworkRpcManager.instance.RemovePlayerStatModifireServerRpc((int)playerNetworkType, (int)statType, value);

    private void RemoveStatModifire(int networkType, int statType, float value)
    {
        PlayerStatType type = (PlayerStatType)statType;
        PlayerNetworkType playerNetworkType = (PlayerNetworkType)networkType;

        switch (type)
        {
            case PlayerStatType.Atk:
                RemoveAtkModifire(playerNetworkType, value);
                break;
            case PlayerStatType.AtkSpeed:
                RemoveAtkSpeedModifire(playerNetworkType, value);
                break;
            case PlayerStatType.AtkRange:
                RemoveAtkRangeModifire(playerNetworkType, value);
                break;
            case PlayerStatType.CriticalRate:
                RemoveCriticalRateModifire(playerNetworkType, value);
                break;
            case PlayerStatType.CriticalDamage:
                RemoveCriDamageModifire(playerNetworkType, value);
                break;
            case PlayerStatType.SkillAcc:
                RemoveSkillAccModifire(playerNetworkType, value);
                break;
            case PlayerStatType.BuffBonus:
                RemoveBuffBonusModifire(playerNetworkType, value);
                break;
            case PlayerStatType.BuffRangeBonus:
                RemoveBuffRangeBonusModifire(playerNetworkType, value);
                break;
        }
    }

    private void AddAtkModifire(PlayerNetworkType playerNetworkType, float value)
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

    private void RemoveAtkModifire(PlayerNetworkType playerNetworkType, float value)
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


    private void AddAtkSpeedModifire(PlayerNetworkType playerNetworkType, float value)
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
    private void RemoveAtkSpeedModifire(PlayerNetworkType playerNetworkType, float value)
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

    private void AddSkillAccModifire(PlayerNetworkType playerNetworkType, float value)
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

    private void RemoveSkillAccModifire(PlayerNetworkType playerNetworkType, float value)
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

    private void AddCriticalRateModifire(PlayerNetworkType playerNetworkType, float value)
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

    private void RemoveCriticalRateModifire(PlayerNetworkType playerNetworkType, float value)
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

    private void AddCriDamageModifire(PlayerNetworkType playerNetworkType, float value)
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

    private void RemoveCriDamageModifire(PlayerNetworkType playerNetworkType, float value)
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


    private void AddAtkRangeModifire(PlayerNetworkType playerNetworkType, float value)
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

    private void RemoveAtkRangeModifire(PlayerNetworkType playerNetworkType, float value)
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

    private void AddGoudBonusModifire(PlayerNetworkType playerNetworkType, float value)
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

    private void RemoveGoldBonusModifire(PlayerNetworkType playerNetworkType, float value)
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

    private void AddBuffBonusModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverBuffBonus.AddModifire(value);
                break;
            case PlayerNetworkType.CLIENT:
                clientBuffBonus.AddModifire(value);
                break;
        }
    }



    private void RemoveBuffBonusModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverBuffBonus.RemoveModifire(value);
                break;
            case PlayerNetworkType.CLIENT:
                clientBuffBonus.RemoveModifire(value);
                break;
        }
    }


    private void AddBuffRangeBonusModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverBuffRangeBounus += value;
                break;
            case PlayerNetworkType.CLIENT:
                clientBuffRangeBounus += value;
                break;
        }
    }



    private void RemoveBuffRangeBonusModifire(PlayerNetworkType playerNetworkType, float value)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverBuffRangeBounus -= value;
                break;
            case PlayerNetworkType.CLIENT:
                clientBuffRangeBounus -= value;
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


    public float GetBuffBonus(PlayerNetworkType playerNetworkType)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                return serverBuffBonus.GetModifireValue();
            case PlayerNetworkType.CLIENT:
                return clientBuffBonus.GetModifireValue();
        }
        return 0;
    }

    public float GetBuffRangeBonus(PlayerNetworkType playerNetworkType)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                return serverBuffRangeBounus;
            case PlayerNetworkType.CLIENT:
                return clientBuffRangeBounus;
        }
        return 0;
    }


    #endregion



}

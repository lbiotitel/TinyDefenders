using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public enum PlayerStatType
{
    Atk,
    AtkSpeed,
    AtkRange,
    CriticalRate,
    CriticalDamage,
    SkillAcc,
    BuffBonus,
    BuffRangeBonus,
    Last
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
    [field: SerializeField] public int maxHeroCount { get; set; }
    [field: SerializeField] public int maxHeroLevel { get; private set; }
    [field: SerializeField] public int requireCombineHeroCount { get; set; }
    [field :SerializeField] public int drawHeroCardCount { get; set; } = 3;
    [field: SerializeField] public int drawSpellCount { get; set; } = 2;
    [field: SerializeField] public int rerollCost { get; private set; } = 5;

    [SerializeField] private int giveExpAmount;
    [SerializeField] private int expGold;

    [Header("Server Bonus Stat")]
    private Dictionary<PlayerStatType, Stat> serverStatDic = new Dictionary<PlayerStatType, Stat>();
    private Dictionary<HeroClass, Dictionary<PlayerStatType, Stat>> serverClassStatDic = new Dictionary<HeroClass, Dictionary<PlayerStatType, Stat>>();
    private Dictionary<HeroTribe, Dictionary<PlayerStatType, Stat>> serverTribeStatDic = new Dictionary<HeroTribe, Dictionary<PlayerStatType, Stat>>();


    [Header("Client Bonus Stat")]
    private Dictionary<PlayerStatType, Stat> clientStatDic = new Dictionary<PlayerStatType, Stat>();
    private Dictionary<HeroClass, Dictionary<PlayerStatType, Stat>> clientClassStatDic = new Dictionary<HeroClass, Dictionary<PlayerStatType, Stat>>();
    private Dictionary<HeroTribe, Dictionary<PlayerStatType, Stat>> clientTribeStatDic = new Dictionary<HeroTribe, Dictionary<PlayerStatType, Stat>>();


    private Dictionary<string, HeroSynergySOBase> clientHeroSynergyDic = new Dictionary<string, HeroSynergySOBase>();
    private Dictionary<string, HeroSynergySOBase> serverHeroSynergyDic = new Dictionary<string, HeroSynergySOBase>();

    private void Start()
    {
        InitHeroSynergy();
        InitPlayerStatDic();

        if (NetUtils.IsHost())
        {
            EventManager.instance.onHeroSpawnAction += AddHero;
            EventManager.instance.onHeroDespawnAction += RemoveHero;
        }

        EventManager.instance.onPlayerAddStatModifireAction += AddStatModifire;
        EventManager.instance.onClassAddStatModifireAction += AddClassStatModifire;
        EventManager.instance.onTribeAddStatModifireAction += AddTribeStatModifire;

        EventManager.instance.onPlayerRemoveStatModifireAction += RemoveStatModifire;
        EventManager.instance.onClassRemoveStatModifireAction += RemoveClassStatModifire;
        EventManager.instance.onTribeRemoveStatModifireAction += RemoveTribeStatModifire;
    }


    #region User Stat

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
        currentMaxHeroCount = Mathf.Min(++currentMaxHeroCount, maxHeroCount);
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

        if (NetUtils.IsHost())
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

        if (NetUtils.IsHost())
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

            NetworkRpcManager.instance.GetHeroSynergyDatasClientRpc(synergyName, heroCount, heroSynergyLevel);
        }
    }


    public void SetClientHeroSynergyData(PlayerNetworkType playerNetworkType, string synergyName, int heroCount, int synergyLevel)
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

    #region About Player Stat 

    private void InitPlayerStatDic()
    {
        InitClassStatDic(serverClassStatDic);
        InitClassStatDic(clientClassStatDic);

        InitTribeStatDic(serverTribeStatDic);
        InitTribeStatDic(clientTribeStatDic);

        InitStatDic(serverStatDic);
        InitStatDic(clientStatDic);
    }

    private void InitClassStatDic(Dictionary<HeroClass, Dictionary<PlayerStatType, Stat>> dic)
    {
        for (int i = (int)HeroClass.WARRIOR; i < (int)HeroClass.LAST; i++)
        {
            dic.Add((HeroClass)i, new Dictionary<PlayerStatType, Stat>());
            InitStatDic(dic[(HeroClass)i]);
        }
    }

    private void InitTribeStatDic(Dictionary<HeroTribe, Dictionary<PlayerStatType, Stat>> dic)
    {
        for (int i = (int)HeroTribe.HUMAN; i < (int)HeroTribe.LAST; i++)
        {
            dic.Add((HeroTribe)i, new Dictionary<PlayerStatType, Stat>());
            InitStatDic(dic[(HeroTribe)i]);
        }
    }

    private void InitStatDic(Dictionary<PlayerStatType, Stat> dic)
    {
        for (int i = (int)PlayerStatType.Atk; i < (int)PlayerStatType.Last; i++)
            dic.Add((PlayerStatType)i, new Stat());
    }


    public void AddStatModifire(PlayerNetworkType playerNetworkType, PlayerStatType statType, float value) 
        => NetworkRpcManager.instance.AddPlayerStatModifireServerRpc((int)playerNetworkType, (int)statType, value);
    public void AddStatModifire(PlayerNetworkType playerNetworkType, HeroClass heroClass, PlayerStatType statType, float value)
        => NetworkRpcManager.instance.AddClassStatModifireServerRpc((int)playerNetworkType,(int)heroClass,(int)statType,value);
    
    public void AddStatModifire(PlayerNetworkType playerNetworkType, HeroTribe heroTribe, PlayerStatType statType, float value)
        => NetworkRpcManager.instance.AddTribeStatModifireServerRpc((int)playerNetworkType, (int)heroTribe, (int)statType, value);


    private void AddStatModifire(int networkType, int statType, float value)
    {
        PlayerStatType type = (PlayerStatType)statType;
        PlayerNetworkType playerNetworkType = (PlayerNetworkType)networkType;

        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                if (serverStatDic.ContainsKey(type))
                    serverStatDic[type].AddModifire(value);
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;

            case PlayerNetworkType.CLIENT:
                if (clientStatDic.ContainsKey(type))
                    clientStatDic[type].AddModifire(value);
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;
        }
    }

    private void AddClassStatModifire(int networkType,int heroClass,int statType, float value)
    {
        PlayerStatType type = (PlayerStatType)statType;
        PlayerNetworkType playerNetworkType = (PlayerNetworkType)networkType;
        HeroClass hClass = (HeroClass)heroClass;

        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                if (serverClassStatDic.ContainsKey(hClass))
                {
                    if (serverClassStatDic[hClass].ContainsKey(type))
                    {
                        serverClassStatDic[hClass][type].AddModifire(value);
                    }
                    else {
                        Debug.LogError("등록되지 않은 스탯타입");
                    }
                }
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;

            case PlayerNetworkType.CLIENT:
                if (clientClassStatDic.ContainsKey(hClass))
                {
                    if (clientClassStatDic[hClass].ContainsKey(type))
                    {
                        clientClassStatDic[hClass][type].AddModifire(value);
                    }
                    else
                    {
                        Debug.LogError("등록되지 않은 스탯타입");
                    }
                }
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;
        }
    }

    private void AddTribeStatModifire(int networkType, int heroTribe, int statType, float value)
    {
        PlayerStatType type = (PlayerStatType)statType;
        PlayerNetworkType playerNetworkType = (PlayerNetworkType)networkType;
        HeroTribe hTribe = (HeroTribe)heroTribe;

        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                if (serverTribeStatDic.ContainsKey(hTribe))
                {
                    if (serverTribeStatDic[hTribe].ContainsKey(type))
                    {
                        serverTribeStatDic[hTribe][type].AddModifire(value);
                    }
                    else
                    {
                        Debug.LogError("등록되지 않은 스탯타입");
                    }
                }
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;

            case PlayerNetworkType.CLIENT:
                if (clientTribeStatDic.ContainsKey(hTribe))
                {
                    if (clientTribeStatDic[hTribe].ContainsKey(type))
                    {
                        clientTribeStatDic[hTribe][type].AddModifire(value);
                    }
                    else
                    {
                        Debug.LogError("등록되지 않은 스탯타입");
                    }
                }
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;
        }
    }


    public void RemoveStatModifire(PlayerNetworkType playerNetworkType, PlayerStatType statType, float value) 
        => NetworkRpcManager.instance.RemovePlayerStatModifireServerRpc((int)playerNetworkType, (int)statType, value);
    public void RemoveStatModifire(PlayerNetworkType playerNetworkType, HeroClass heroClass, PlayerStatType statType, float value)
    => NetworkRpcManager.instance.RemoveClassStatModifireServerRpc((int)playerNetworkType, (int)heroClass, (int)statType, value);

    public void RemoveStatModifire(PlayerNetworkType playerNetworkType, HeroTribe heroTribe, PlayerStatType statType, float value)
        => NetworkRpcManager.instance.RemoveTribeStatModifireServerRpc((int)playerNetworkType, (int)heroTribe, (int)statType, value);


    private void RemoveStatModifire(int networkType, int statType, float value)
    {
        PlayerStatType type = (PlayerStatType)statType;
        PlayerNetworkType playerNetworkType = (PlayerNetworkType)networkType;

        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                if (serverStatDic.ContainsKey(type))
                    serverStatDic[type].RemoveModifire(value);
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;

            case PlayerNetworkType.CLIENT:
                if (clientStatDic.ContainsKey(type))
                    clientStatDic[type].RemoveModifire(value);
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;
        }
    }

    private void RemoveClassStatModifire(int networkType, int heroClass, int statType, float value)
    {
        PlayerStatType type = (PlayerStatType)statType;
        PlayerNetworkType playerNetworkType = (PlayerNetworkType)networkType;
        HeroClass hClass = (HeroClass)heroClass;

        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                if (serverClassStatDic.ContainsKey(hClass))
                {
                    if (serverClassStatDic[hClass].ContainsKey(type))
                    {
                        serverClassStatDic[hClass][type].RemoveModifire(value);
                    }
                    else
                    {
                        Debug.LogError("등록되지 않은 스탯타입");
                    }
                }
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;

            case PlayerNetworkType.CLIENT:
                if (clientClassStatDic.ContainsKey(hClass))
                {
                    if (clientClassStatDic[hClass].ContainsKey(type))
                    {
                        clientClassStatDic[hClass][type].RemoveModifire(value);
                    }
                    else
                    {
                        Debug.LogError("등록되지 않은 스탯타입");
                    }
                }
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;
        }
    }



    private void RemoveTribeStatModifire(int networkType, int heroTribe, int statType, float value)
    {
        PlayerStatType type = (PlayerStatType)statType;
        PlayerNetworkType playerNetworkType = (PlayerNetworkType)networkType;
        HeroTribe hTribe = (HeroTribe)heroTribe;

        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                if (serverTribeStatDic.ContainsKey(hTribe))
                {
                    if (serverTribeStatDic[hTribe].ContainsKey(type))
                    {
                        serverTribeStatDic[hTribe][type].RemoveModifire(value);
                    }
                    else
                    {
                        Debug.LogError("등록되지 않은 스탯타입");
                    }
                }
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;

            case PlayerNetworkType.CLIENT:
                if (clientTribeStatDic.ContainsKey(hTribe))
                {
                    if (clientTribeStatDic[hTribe].ContainsKey(type))
                    {
                        clientTribeStatDic[hTribe][type].RemoveModifire(value);
                    }
                    else
                    {
                        Debug.LogError("등록되지 않은 스탯타입");
                    }
                }
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;
        }
    }




    #endregion

    #region About Get Player Stat

    public float GetStatModifire(PlayerNetworkType playerNetworkType, PlayerStatType statType)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                if (serverStatDic.ContainsKey(statType))
                    return serverStatDic[statType].GetModifireValue();
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;

            case PlayerNetworkType.CLIENT:
                if (clientStatDic.ContainsKey(statType))
                    return clientStatDic[statType].GetModifireValue();
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;
        }

        return 0;
    }

    public float GetStatModifire(PlayerNetworkType playerNetworkType, HeroClass heroClass, HeroTribe heroTribe ,PlayerStatType statType)
    {
        float v1 = GetStatModifire(playerNetworkType,statType);
        float v2 = GetStatModifire(playerNetworkType,heroClass,statType);
        float v3 = GetStatModifire(playerNetworkType,heroTribe,statType);


        return v1 + v2 + v3;
    }

    public float GetStatModifire(PlayerNetworkType playerNetworkType, HeroClass heroClass ,PlayerStatType statType)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                if (serverClassStatDic.ContainsKey(heroClass))
                {
                    if (serverClassStatDic[heroClass].ContainsKey(statType))
                    {
                        return serverClassStatDic[heroClass][statType].GetModifireValue();
                    }
                    else
                    {
                        Debug.LogError("등록되지 않은 스탯타입");
                    }
                }
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;

            case PlayerNetworkType.CLIENT:
                if (clientClassStatDic.ContainsKey(heroClass))
                {
                    if (clientClassStatDic[heroClass].ContainsKey(statType))
                    {
                        return clientClassStatDic[heroClass][statType].GetModifireValue();
                    }
                    else
                    {
                        Debug.LogError("등록되지 않은 스탯타입");
                    }
                }
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;
        }

        return 0;
    }

    public float GetStatModifire(PlayerNetworkType playerNetworkType, HeroTribe heroTribe, PlayerStatType statType)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                if (serverTribeStatDic.ContainsKey(heroTribe))
                {
                    if (serverTribeStatDic[heroTribe].ContainsKey(statType))
                    {
                        return serverTribeStatDic[heroTribe][statType].GetModifireValue();
                    }
                    else
                    {
                        Debug.LogError("등록되지 않은 스탯타입");
                    }
                }
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;

            case PlayerNetworkType.CLIENT:
                if (clientTribeStatDic.ContainsKey(heroTribe))
                {
                    if (clientTribeStatDic[heroTribe].ContainsKey(statType))
                    {
                        return clientTribeStatDic[heroTribe][statType].GetModifireValue();
                    }
                    else
                    {
                        Debug.LogError("등록되지 않은 스탯타입");
                    }
                }
                else
                    Debug.LogError("등록되지 않은 스탯타입");
                break;
        }

        return 0;
    }

    #endregion



}

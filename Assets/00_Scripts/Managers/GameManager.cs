using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;


public delegate void OnGoldGetEventHandler();
public delegate void OnTimerUpEventHandler(string monsterName);
public delegate void OnGameOver();
public class GameManager : NetworkBehaviour
{
    public static GameManager instance = null;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    //config
    [field: SerializeField] public int maxMonsterCount { get; private set; }
    [field: SerializeField] public int maxHeroCount { get; private set; }
    [field: SerializeField] public int gold { get; set; }
    [field: SerializeField] public int summonGold { get; set; }
    [field: SerializeField] public int summonGoldInc { get; set; }

    public int currentWave = 0;
    public float timer = 100000;
    public int currentMonsterCount = 0;

    public int currentHeroCount = 0;
    public int[] upgarde = new int[4];
    public int goldGoblinLevel = 1;
    public int goldGoblinRewardGold = 100;
    public float goldGoblinDelayTime = 0f;
    private bool goldGoblinDelay = false;

    public int warningMonsterCount;
    public float warningAlarmDelayTime = 30f;
    private bool warningAlarmDelay = false;

    private bool isGameStart = false;

    public double playerDps = 0f;
    public double friendDps = 0f;

    //event
    public event OnGoldGetEventHandler onGoldGet;
    public event OnTimerUpEventHandler onTimerUp;
    public event OnGameOver onGameOver;

    private List<Monster> monsterList = new List<Monster>();

    private WaveScriptable[] waveData = null;

    private List<Hero> clientHeroList = new List<Hero>();
    private List<Hero> serverHeroList = new List<Hero>();



    private void Update()
    {
        CheckEndProfileAnimation();
        UpdateTime();
    }

    #region Game Start Logic
    private void InitGameManager()
    {
        if (IsServer)
        {
            waveData = Resources.LoadAll<WaveScriptable>("WaveDatas");
            waveData = waveData.OrderBy(wave => int.Parse(wave.name)).ToArray();
            StartWave();
        }
    }


    public void SetSpawner()
    {
        Spawner.instance.SetSpawner();
    }

    public void StartGame()
    {
        isGameStart = true;
        CardManager.instance.RerollCard(true);
        SoundManager.instance.PlayMusicSound("InGameBgm", true);
    }



    private void CheckEndProfileAnimation()
    {
        if (!isGameStart) return;
        if (InGameUI.instance.profileAnimationReady)
        {
            InGameUI.instance.profileAnimationReady = false;
            InGameUI.instance.HideProfileAnimation();
            if (IsServer) InitGameManager();
        }
    }

    #endregion


    #region Game Wave Logic
    private void UpdateTime()
    {
        if (!isGameStart) return;
        if (waveData == null) return;
        if (IsServer)
        {
            bool waveUI = false;
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                timer = Mathf.Max(timer, 0);
            }
            else
            {
                waveUI = true;
                StartWave();
            }
            NotifyTimerClientRpc(
                timer,
                currentWave,
                waveUI,
                waveData[currentWave - 1].bossWave,
                waveData[currentWave - 1].spawnMonsterData.monsterName);
        }
    }

    private void StartWave()
    {
        currentWave = Mathf.Min(++currentWave, waveData.Length);

        timer = waveData[currentWave - 1].waveTime;
        Spawner.instance.StartMonsterSpawn(
            waveData[currentWave - 1].spawnMonsterData.monsterName,
            waveData[currentWave - 1].spawnInterval,
            waveData[currentWave - 1].waveMonsterCount);
    }

    [ClientRpc]
    private void NotifyTimerClientRpc(float time, int wave, bool waveUI, bool isBoss, string monsterName)
    {
        this.timer = time;
        this.currentWave = wave;
        if (waveUI)
        {
            if (!isBoss)
                InGameUI.instance.ShowWaveUI();
            else
                InGameUI.instance.ShowBossWaveUI(monsterName);
        }
        onTimerUp?.Invoke(monsterName);
    }

    public void BossMonsterDead()
    {
        if (timer > 0)
        {
            StartWave();
            NotifyTimerClientRpc(
                 timer,
                 currentWave,
                 true,
                 waveData[currentWave - 1].bossWave,
                 waveData[currentWave - 1].spawnMonsterData.monsterName);
        }
        else
        {
            GameOver();
        }
    }


    #endregion


    #region Monster Count Method

    private void UpdateClientMonsterCount()
    {
        NotifyClientMonsterCountClientRpc(currentMonsterCount);
    }

    public void AddMonster(Monster monster)
    {
        monsterList.Add(monster);
        currentMonsterCount++;
        CheckMonsterCount();
        UpdateClientMonsterCount();
    }
    public void RemoveMonster(Monster monster)
    {
        monsterList.Remove(monster);
        currentMonsterCount--;
        UpdateClientMonsterCount();
    }

    private void CheckMonsterCount()
    {
        if (NetUtils.IsHost())
        {
            if (warningMonsterCount <= currentMonsterCount) ShowWarningAlarm();
            if (currentMonsterCount >= maxMonsterCount) GameOver();
        }
    }

    private void ShowWarningAlarm()
    {
        if (!warningAlarmDelay)
        {
            warningAlarmDelay = true;

            NetUtils.HostAndClientMethod(() =>
            {
                InGameUI.instance.ShowAlarmServerRpc("몬수터 수 위험!", 2f, AlarmType.WARNING.ToString());
            }, () =>
            {
                InGameUI.instance.ShowAlarmClientRpc("몬수터 수 위험!", 2f, AlarmType.WARNING.ToString());
            });

            StartCoroutine(WarningAlramCoroutine());
        }
    }

    private IEnumerator WarningAlramCoroutine()
    {
        yield return new WaitForSeconds(warningAlarmDelayTime);
        warningAlarmDelay = false;
    }



    [ClientRpc]
    private void NotifyClientMonsterCountClientRpc(int count)
    {
        currentMonsterCount = count;
    }



    #endregion



    #region About Gold
    public void GetGold(int gold, PlayerNetworkType playerNetworkType = PlayerNetworkType.ALL)
    {
        GiveGoldToAllPlayersClientRpc(playerNetworkType, gold);
    }

    [ClientRpc]
    private void GiveGoldToAllPlayersClientRpc(PlayerNetworkType playerNetworkType, int gold)
    {
        if (playerNetworkType == PlayerNetworkType.ALL)
        {
            if (NetUtils.GetPlayerNetworkType() == PlayerNetworkType.HOST) GetGold(gold);
            if (NetUtils.GetPlayerNetworkType() == PlayerNetworkType.CLIENT) GetGold(gold);
        }
        else if (NetUtils.GetPlayerNetworkType() == PlayerNetworkType.CLIENT)
        {
            GetGold(gold);
        }
        else if (NetUtils.GetPlayerNetworkType() == PlayerNetworkType.HOST)
        {
            GetGold(gold);
        }
    }


    public void GetGold(int gold)
    {
        this.gold += (int)(gold + (gold * PlayerStatManager.instance.GetBonusGold(NetUtils.GetPlayerNetworkType())));
    }

    public bool IsEnoughGold(int gold)
    {
        if (this.gold < gold) return false;
        return true;
    }

    #endregion




    #region Game Over Logic
    private void CleanNetworkObject()
    {
        if (IsServer)
        {
            foreach (var obj in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
            {
                if (obj != null && obj.IsSpawned)
                {
                    obj.Despawn(true);
                }
            }
        }
    }

    public void GameOver()
    {
        GameOverClientRpc(playerDps, friendDps);
        StartCoroutine(ChangeToMainSceneForAllPlayers());
    }

    [ClientRpc]
    private void GameOverClientRpc(double playerDps, double friendDps)
    {
        CloudManager.instance.userData.bestWave = currentWave;
        CloudManager.instance.SaveUserData();
        Time.timeScale = 0f;
        onGameOver?.Invoke();

        if (!IsServer)
        {
            this.playerDps = playerDps;
            this.friendDps = friendDps;
        }
    }

    private IEnumerator ChangeToMainSceneForAllPlayers()
    {
        yield return new WaitForSecondsRealtime(3f);
        CleanNetworkObject();
        Time.timeScale = 1f;
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                SceneName.MainScene,
                UnityEngine.SceneManagement.LoadSceneMode.Single
            );
        }
    }

    #endregion



    #region About Hero
    public void AddHero(Hero hero)
    {
        PlayerNetworkType playerNetworkType = NetUtils.GetPlayerNetworkType();

        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverHeroList.Add(hero);
                break;
            case PlayerNetworkType.CLIENT:
                clientHeroList.Add(hero);
                break;
        }
        EventManager.instance.onHeroSummonAction?.Invoke();
    }

    public void RemoveHero(Hero hero)
    {
        PlayerNetworkType playerNetworkType = NetUtils.GetPlayerNetworkType();

        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverHeroList.Remove(hero);
                break;
            case PlayerNetworkType.CLIENT:
                clientHeroList.Remove(hero);
                break;
        }
        EventManager.instance.onHeroDespawnAction?.Invoke();
    }

    public List<Hero> GetHeroList(PlayerNetworkType playerNetworkType)
    {
        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                return serverHeroList;
            case PlayerNetworkType.CLIENT:
                return clientHeroList;
        }

        return null;
    }

    #endregion

    public void SummonGoldGoblin()
    {
        if (!goldGoblinDelay)
        {
            goldGoblinDelay = true;
            goldGoblinLevel++;

            NetUtils.HostAndClientMethod(() =>
            {
                InGameUI.instance.ShowAlarmServerRpc("황금 고블린 소환!", 3f, AlarmType.GOOD.ToString());
                Spawner.instance.StartMonsterSpawnServerRpc("GoldGoblin", 0, 1);
            }, () =>
            {
                InGameUI.instance.ShowAlarmClientRpc("황금 고블린 소환!", 3f, AlarmType.GOOD.ToString());
                Spawner.instance.StartMonsterSpawn("GoldGoblin", 0, 1);
            });
        }
    }

    #region Send Name
    //send users name
    [ServerRpc(RequireOwnership = false)]
    public void SendUserNameServerRpc(string userName)
    {
        if (IsServer)
        {
            SendUserNameClientRpc(CloudManager.instance.userData.userName, userName);
        }
    }

    [ClientRpc]
    private void SendUserNameClientRpc(string hostName, string clientName)
    {
        InGameUI.instance.InitAllUserName(hostName, clientName);
    }
    #endregion


}

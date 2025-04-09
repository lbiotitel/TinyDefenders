using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


public delegate void OnTimerUpEventHandler(string monsterName);
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
    [field: SerializeField] public int gold { get; set; }

    private int currentWave = 0;
    public float timer = 100000;
    public int currentMonsterCount = 0;

    public int warningMonsterCount;
    public float warningAlarmDelayTime = 30f;
    private bool warningAlarmDelay = false;

    private bool isGameStart = false;
    public bool isGameOver { get; private set; } = false;

    private string playerName;
    private string friendName;
    public double playerDamage = 0f;
    public double friendDamage = 0f;

    //event
    public event OnTimerUpEventHandler onTimerUp;

    private List<Monster> monsterList = new List<Monster>();

    private WaveScriptable[] waveData = null;


    private void Update()
    {
        CheckEndProfileAnimation();
        WaveLogic();
    }

    #region Game Start Logic
    private void InitGameManager()
    {
        if (IsServer)
        {
            waveData = Resources.LoadAll<WaveScriptable>(Path.forestWavePath);
            waveData = waveData.OrderBy(wave => int.Parse(wave.name)).ToArray();
            StartWave();
        }
    }


    public void SetSpawner()
    {
        SpawnManager.instance.SetSpawner();
    }

    public void StartGame()
    {
        isGameStart = true;
        CardManager.instance.RerollCard(true);
        SoundManager.instance.PlayMusicSound("InGameBgm", true);
        EventManager.instance.onGameStartAction?.Invoke();
    }

    public void InitAllUserName(string hostName, string clientName)
    {
        playerName = hostName;
        friendName = clientName;
        InGameUI.instance.InitAllUserName(hostName, clientName);
    }

    public void InitRelicDatas(Dictionary<string,bool> ownRelicData)
    {
        List<RelicSO> relicList = SoDataManager.instance.GetAllRelicData();

        foreach (var relic in relicList)
        {
            if (ownRelicData.ContainsKey(relic.relicName))
            { 
                bool isActive = ownRelicData[relic.relicName];
                if (isActive)
                {
                    relic.InitRelicSO(NetUtils.GetPlayerNetworkType());
                }
            }
        }
    }


    private void CheckEndProfileAnimation()
    {
        if (!isGameStart) return;
        if (InGameUI.instance.profileAnimationReady)
        {
            InGameUI.instance.profileAnimationReady = false;
            InGameUI.instance.HideProfileAnimation();
            CheckEndProfileAnimationServerRpc((int)NetUtils.GetPlayerNetworkType());
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void CheckEndProfileAnimationServerRpc(int networkType)
    {
        PlayerNetworkType playerNetworkType = (PlayerNetworkType)networkType;

        if (playerNetworkType == PlayerNetworkType.CLIENT)
        {
            if(NetUtils.IsHost())StartCoroutine(ShowCountDown());
        }
    }

    private IEnumerator ShowCountDown()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 3; i >= 1; i--)
        {
            ShowCountDownClientRpc(i);
            if(i == 1)yield return new WaitForSeconds(0.8f);
            else yield return new WaitForSeconds(1f);
        }

        ShowCountDownClientRpc(0);

        if (NetUtils.IsHost()) InitGameManager();
    }

    [ClientRpc]
    private void ShowCountDownClientRpc(int count)
    {
        if (count == 0)
        {
            InGameUI.instance.HideCountDownText();
            return;
        }
        InGameUI.instance.ShowCountDownText(count);
    }

    #endregion


    #region Game Wave Logic
    private void WaveLogic()
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
        SpawnManager.instance.StartMonsterSpawn(
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

    public int GetCurrentWave()
    { 
        return currentWave;
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
            GetGold(gold);
        }
        else
        {
            if (NetUtils.GetPlayerNetworkType() == playerNetworkType)
            {
                GetGold(gold);
            }
            else if (NetUtils.GetPlayerNetworkType() == playerNetworkType)
            {
                GetGold(gold);
            }
        }
    }


    public void GetGold(int gold)
    {
        this.gold += (int)(gold + (gold * (PlayerStatManager.instance.GetBonusGold(NetUtils.GetPlayerNetworkType()) / 100f)));
        InGameUI.instance.ShowGetGoldText(gold);
    }

    public bool IsEnoughGold(int gold)
    {
        if (this.gold < gold) return false;
        return true;
    }

    public void SpendGold(int gold)
    {
        this.gold -= gold;
    }

    #endregion




    #region Game Over Logic

    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            if (NetUtils.IsHost())
            {
                GameStopClientRpc();
                //나중에 playerstatemanager로 뺌
                NetworkRpcManager.instance.GameFinishClientRpc(playerName, friendName, playerDamage, friendDamage, currentWave);
            }
        }
    }

    [ClientRpc]
    private void GameStopClientRpc()
    {
        SoundManager.instance.FadeOutSound();
        StartCoroutine(GameStopCoroutine());
    }

    private IEnumerator GameStopCoroutine()
    {
        float elapsedTime = 0f;
        float gameStopTime = 2f;
        float percent = 0f;
        while (percent < 1f)
        {
            elapsedTime -= Time.deltaTime;
            percent = elapsedTime / gameStopTime;
            Time.timeScale = Mathf.Lerp(Time.timeScale,0,percent);
            yield return null;
        }
    }

    public void ChangeAllPlayerToResultScene()
    {
        NetworkLobbyManager.instance.ReleaseEvent();
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(SceneName.ResultScene);
    }


    #endregion

}

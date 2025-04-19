using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    [field: SerializeField] public int gold { get; set; }

    private int currentWave = 0;
    public float timer = 100000;
    public float selectRelicTime = 20f;
    public int prepareTime = 20;
    public int gameLife;

    public int warningTime;
    public float warningAlarmDelayTime = 30f;
    private bool warningAlarmDelay = false;

    private bool isGameStart = false;
    public bool isGameOver { get; private set; } = false;

    private string playerName;
    private string friendName;
    public double playerDamage = 0f;
    public double friendDamage = 0f;


    private WaveScriptable[] waveData = null;
    private WaveScriptable currentWaveData = null;
    private const int bossKillCount = 2;
    private int currentbossKillCount = 0;
    private string mapName;


    private void Start()
    {
        InitGameManager();
    }


    private void Update()
    {
        CheckEndProfileAnimation();
    }

    #region Game Start Logic
    private void InitGameManager()
    {
        if (IsServer)
        {
            waveData = Resources.LoadAll<WaveScriptable>(Path.wavePath + mapName);
            waveData = waveData.OrderBy(wave => int.Parse(wave.name)).ToArray();

        }
    }

    public void InitMap(string mapName)
    {
        this.mapName = mapName;
    }

    public string GetMapName() { return mapName; }


    public void SetSpawner()
    {
        SpawnManager.instance.SetSpawnManager();
    }

    public void StartGame()
    {
        isGameStart = true;
        CardManager.instance.RerollCard(true);
        SoundManager.instance.PlayMusicSound("InGameBgm", true);
        EventManager.instance.onGameStartAction?.Invoke();
    }

    public void InitAllUserInfo(string hostName, string clientName)
    {
        playerName = hostName;
        friendName = clientName;
        InGameUI.instance.InitAllUserName(hostName, clientName);
    }

    public void InitRelicDatas(Dictionary<string, bool> ownRelicData)
    {
        List<RelicSO> relicList = GameResourceManager.instance.GetAllRelicData();

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
            if (NetUtils.IsHost()) StartCoroutine(CountDown(3));
        }
    }

    private IEnumerator CountDown(int time)
    {
        NotifyTimerClientRpc(0, currentWave, true);
     
        yield return new WaitForSeconds(1f);

        for (int i = time; i >= 1; i--)
        {
            NotifyTimerClientRpc(i,currentWave,true);
            if (i <= 3)
            {
                ShowCountDownClientRpc(i);
                if (i == 1) yield return new WaitForSeconds(0.8f);
            }
            yield return new WaitForSeconds(1f);
        }

        ShowCountDownClientRpc(0);

        if (NetUtils.IsHost()) StartWave();
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


    private void StartWave()
    {
        currentWave = Mathf.Min(++currentWave, waveData.Length);
        currentWaveData = waveData[currentWave - 1];
        float waveTime = currentWaveData.waveTime;

        SpawnManager.instance.StartMonsterSpawn(
            currentWaveData.spawnMonsterData.monsterName,
            currentWaveData.spawnInterval,
            currentWaveData.waveMonsterCount);

        StartCoroutine(WaveTimeCoroutine(waveTime));
    }

    private IEnumerator WaveTimeCoroutine(float waveTime)
    {
        timer = waveTime;
        NotifyTimerClientRpc(timer, currentWave);
        ShowWaveUIClientRpc(currentWaveData.bossWave, currentWaveData.spawnMonsterData.monsterName);

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            NotifyTimerClientRpc(timer, currentWave);

            yield return null;
        }

        CheckClearWave();
        StartCoroutine(CountDown(prepareTime));
    }

    private void CheckClearWave()
    {
        if (NetUtils.IsHost())
        {
            if (SpawnManager.instance.GetMonsterCount() > 0)
            {
                gameLife--;
                if (gameLife <= 0) GameOver();
                else
                {
                    InGameUI.instance.RedrawLifeContainerClientRpc(gameLife);
                    WaveFailEventCalledClientRpc();
                }
            }
            else {
                ShowClearAlaram();
                GetGold(currentWaveData.waveClearGold,PlayerNetworkType.ALL);
                WaveClearEventCalledClientRpc();
            }
        }
    }

    [ClientRpc]
    private void WaveFailEventCalledClientRpc() => EventManager.instance.onWaveNotClearAction?.Invoke();
    [ClientRpc]
    private void WaveClearEventCalledClientRpc() => EventManager.instance.onWaveClearAction?.Invoke();


    [ClientRpc]
    private void NotifyTimerClientRpc(float time, int wave, bool prepare = false)
    {
        time = Mathf.Max(0,time);
        this.timer = time;
        this.currentWave = wave;
        EventManager.instance.onTimerTickAction?.Invoke(time,prepare);
    }

    [ClientRpc]
    private void ShowWaveUIClientRpc(bool isBoss, string monsterName)
    {
        if (!isBoss)
            InGameUI.instance.ShowWaveUI();
        else
            InGameUI.instance.ShowBossWaveUI(monsterName);
    }


    public void BossMonsterDead()
    {
        currentbossKillCount++;
        if (currentbossKillCount == bossKillCount)
        {
            NetworkRpcManager.instance.BossClearEventClientRpc();
            StartCoroutine(SelectRelicCoroutine());
        }
    }


    private IEnumerator SelectRelicCoroutine()
    {
        yield return new WaitForSeconds(selectRelicTime);

        StartWave();
        NotifyTimerClientRpc(timer, currentWave);

        currentbossKillCount = 0;
    }


    public int GetCurrentWave()
    {
        return currentWave;
    }


    #endregion


    #region Monster Count Method


    private void ShowWarningAlarm()
    {
        if (!warningAlarmDelay)
        {
            Debug.Log("호출");
            warningAlarmDelay = true;

            NetUtils.HostAndClientMethod(() =>
            {
                InGameUI.instance.ShowAlarmServerRpc("남은 시간 10초!", 2f, AlarmType.WARNING.ToString());
            }, () =>
            {
                InGameUI.instance.ShowAlarmClientRpc("남은 시간 10초", 2f, AlarmType.WARNING.ToString());
            });

            StartCoroutine(WarningAlramCoroutine());
        }
    }

    private void ShowClearAlaram()
    {
        NetUtils.HostAndClientMethod(() =>
        {
            InGameUI.instance.ShowAlarmServerRpc("웨이브 클리어!", 2f, AlarmType.GOOD.ToString());
        }, () =>
        {
            InGameUI.instance.ShowAlarmClientRpc("웨이브 클리어!", 2f, AlarmType.GOOD.ToString());
        });

    }

    private IEnumerator WarningAlramCoroutine()
    {
        yield return new WaitForSeconds(warningAlarmDelayTime);
        warningAlarmDelay = false;
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
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0, percent);
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

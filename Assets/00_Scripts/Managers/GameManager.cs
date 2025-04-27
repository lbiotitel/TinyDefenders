using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
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

    private bool hostRelicSelect = false;
    private bool clientRelicSelect = false;

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
        EventManager.instance.onRelicSelectFinishAction += FinishRelicSelectServerRpc;
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
            if (NetUtils.IsHost())
            {
                StartCoroutine(CountDown(prepareTime));
                GameStartEventClientRpc();
            }
        }

    }

    [ClientRpc]
    private void GameStartEventClientRpc() => EventManager.instance.onGameStartAction?.Invoke();

    private IEnumerator CountDown(int time)
    {
        yield return new WaitForSeconds(2f);

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
            if (SpawnManager.instance.GetAliveMonsterCount() > 0)
            {
                gameLife--;
                if (gameLife <= 0)
                {
                    GameOver();
                    return;
                }
                else
                {
                    InGameUI.instance.RedrawLifeContainerClientRpc(gameLife);
                    ShowFailedAlram();
                    WaveFailEventCalledClientRpc();
                }
            }
            else {
                ShowClearAlram();
                WaveClearEventCalledClientRpc();
            }
        }

        GetGold(currentWaveData.waveClearGold, PlayerNetworkType.ALL);
        WaveFinishEventCalledClientRpc();
    }

    [ClientRpc]
    private void WaveFailEventCalledClientRpc() => EventManager.instance.onWaveNotClearAction?.Invoke();
    [ClientRpc]
    private void WaveClearEventCalledClientRpc() => EventManager.instance.onWaveClearAction?.Invoke();
    [ClientRpc]
    private void WaveFinishEventCalledClientRpc() => EventManager.instance.onWaveFinishAction?.Invoke();


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
            currentbossKillCount = 0;
            BossClearEventClientRpc();
            TimeManager.instance.SetTimeMode(TimeMode.FREEZE);
        }
    }

    [ClientRpc]
    public void BossClearEventClientRpc() => EventManager.instance.onBossWaveClear?.Invoke();



    public int GetCurrentWave()
    {
        return currentWave;
    }


    #endregion

    #region Relic Select Logic

    [ServerRpc(RequireOwnership = false)]
    private void FinishRelicSelectServerRpc(PlayerNetworkType playerNetworkType)
    {
        if(playerNetworkType == PlayerNetworkType.HOST) hostRelicSelect = true;
        if(playerNetworkType == PlayerNetworkType.CLIENT) clientRelicSelect = true;

        if (hostRelicSelect && clientRelicSelect)
        {
            if (NetUtils.IsHost())
            {
                FinishRelicSelect();
            }
        }
    }

    private void FinishRelicSelect()
    {
        TimeManager.instance.SetTimeMode(TimeMode.X1);
        hostRelicSelect = false;
        clientRelicSelect = false;
        StopAllCoroutines();
        StartCoroutine(CountDown(prepareTime));
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

    private void ShowFailedAlram()
    {
        NetUtils.HostAndClientMethod(() =>
        {
            InGameUI.instance.ShowAlarmServerRpc("클리어 실패!", 2f, AlarmType.BAD.ToString());
        }, () =>
        {
            InGameUI.instance.ShowAlarmClientRpc("클리어 실패!", 2f, AlarmType.BAD.ToString());
        });

    }

    private void ShowClearAlram()
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
        this.gold += gold;
        InGameUI.instance.ShowGetGoldText(gold);
    }

    public bool IsEnoughGold(int gold)
    {
        if (this.gold < gold)
        {
            AdviceUIManager.instance.AddAdvice("골드가 부족합니다.");
            return false;
        }
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
                GameSoundFadeOutClientRpc();
                TimeManager.instance.SetTimeMode(TimeMode.FREEZE);
                //나중에 playerstatemanager로 뺌
                NetworkRpcManager.instance.GameFinishClientRpc(playerName, friendName, playerDamage, friendDamage, currentWave);
            }
        }
    }

    [ClientRpc]
    private void GameSoundFadeOutClientRpc() => SoundManager.instance.FadeOutSound();


    public void ChangeAllPlayerToResultScene()
    {
        NetworkLobbyManager.instance.ReleaseEvent();
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(SceneName.ResultScene);
    }



    #endregion

}

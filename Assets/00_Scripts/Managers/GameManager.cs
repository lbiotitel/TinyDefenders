using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;


public delegate void OnGoldGetEventHandler();
public delegate void OnTimerUpEventHandler(string monsterName);
public delegate void OnSummonHeroEventHandler();
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
    [SerializeField] private string mainSceneName;

    public int currentWave = 0;
    public float timer = 100000;
    public int currentMonsterCount = 0;
    public int currentHeroCount = 0;
    public int[] upgarde = new int[4];
    public int goldGoblinLevel = 1;
    public int goldGoblinRewardGold = 100;
    public float goldGoblinDelayTime = 0f;
    private bool goldGoblinDelay = false;

    private bool isGameStart = false;

    public double playerDps = 0f;
    public double friendDps = 0f;

    //event
    public event OnGoldGetEventHandler onGoldGet;
    public event OnTimerUpEventHandler onTimerUp;
    public event OnSummonHeroEventHandler onSummonHero;
    public event OnGameOver onGameOver;

    private List<Monster> monsterList = new List<Monster>();

    private WaveScriptable[] waveData = null;

    private List<Hero> heroLists = new List<Hero>();


    private void Start()
    {
        if (IsClient)
        {
            SendUserNameServerRpc(CloudManager.instance.userData.userName);
        }
    }

    private void InitGameManager()
    {
        if (IsServer)
        {
            waveData = Resources.LoadAll<WaveScriptable>("WaveDatas");
            waveData = waveData.OrderBy(wave => int.Parse(wave.name)).ToArray();
            StartWave();
        }
    }


    private void Update()
    {
        CheckEndProfileAnimation();
        UpdateTime();
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


    public void AddMonster(Monster monster)
    {
        monsterList.Add(monster);
        currentMonsterCount++;
        if (currentMonsterCount >= maxMonsterCount)
        {
            GameOver();
        }
        UpdateClientMonsterCount();
    }
    public void RemoveMonster(Monster monster)
    {
        monsterList.Remove(monster);
        currentMonsterCount--;
        UpdateClientMonsterCount();
    }
    public void GetGold(int gold,HostType hostType = HostType.ALL)
    {
        if (hostType == HostType.ALL)
        {
            GiveGoldToAllPlayersClientRpc(gold);
        }
        else if(hostType == HostType.CLIENT) {
            this.gold += gold;
            onGoldGet?.Invoke();
        }
    }

    private void UpdateClientMonsterCount()
    {
        NotifyClientMonsterCountClientRpc(currentMonsterCount);
    }

    public void BossMonsterDead()
    {
        StartWave();
        NotifyTimerClientRpc(
             timer,
             currentWave,
             true,
             waveData[currentWave - 1].bossWave,
             waveData[currentWave - 1].spawnMonsterData.monsterName);
    }

    [ClientRpc]
    private void GiveGoldToAllPlayersClientRpc(int gold)
    {
        this.gold += gold;
        onGoldGet?.Invoke();
    }

    [ClientRpc]
    private void NotifyTimerClientRpc(float time,int wave,bool waveUI,bool isBoss,string monsterName)
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

    public void AddHero(Hero hero)
    {
        heroLists.Add(hero);
        currentHeroCount++;
        onSummonHero?.Invoke();
    }

    public void RemoveHero(Hero hero)
    { 
        heroLists.Remove(hero);
        currentHeroCount--;
        onSummonHero?.Invoke();
    }   


    [ClientRpc]
    private void NotifyClientMonsterCountClientRpc(int count)
    {
        currentMonsterCount = count;
    }

    public void StartGame()
    { 
        isGameStart = true;
        //SoundManager.instance.PlayMusicSound("InGameBgm",true);
    }


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
        GameOverClientRpc(playerDps,friendDps);
        StartCoroutine(ChangeToMainSceneForAllPlayers());
    }

    [ClientRpc]
    private void GameOverClientRpc(double playerDps,double friendDps)
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
                mainSceneName,
                UnityEngine.SceneManagement.LoadSceneMode.Single
                );
        }
    }

    public void SummonGoldGoblin()
    {
        if (!goldGoblinDelay)
        {
            goldGoblinDelay = true;
            goldGoblinLevel++;
            StartCoroutine(GoldGolbinDelayCoroutine());
            

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

    private IEnumerator GoldGolbinDelayCoroutine()
    {
        float currentTime = 0f;

        while (currentTime <= goldGoblinDelayTime)
        {
            currentTime += Time.deltaTime;

            InGameUI.instance.summonGoldGoblinImage.fillAmount = currentTime / goldGoblinDelayTime;

            yield return null;
        }
        goldGoblinDelay = false;
    }


    //send users name
    [ServerRpc(RequireOwnership = false)]
    private void SendUserNameServerRpc(string userName)
    {
        if (IsServer)
        {
            SendUserNameClientRpc(CloudManager.instance.userData.userName,userName);
        }
    }

    [ClientRpc]
    private void SendUserNameClientRpc(string hostName, string clientName)
    {
        InGameUI.instance.InitAllUserName(hostName,clientName);
    }
}

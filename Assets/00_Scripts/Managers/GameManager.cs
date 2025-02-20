using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;


public delegate void OnGoldGetEventHandler();
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
    [field: SerializeField] public int maxHeroCount { get; private set; }
    [field: SerializeField] public int gold { get; set; }
    [field: SerializeField] public int summonGold { get; set; }
    [field: SerializeField] public int summonGoldInc { get; set; }

    public int currentWave = 0;
    public float timer = 100000;
    public int currentMonsterCount = 0;
    public int heroCount = 0;
    public int[] upgarde = new int[4];

    //event
    public event OnGoldGetEventHandler onGoldGet;
    public event OnTimerUpEventHandler onTimerUp;

    private List<Monster> monsterList = new List<Monster>();

    private WaveScriptable[] waveData;


    private void Start()
    {
        InitGameManager();
        Time.timeScale = 0f;
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
        UpdateTime();
    }

    private void UpdateTime()
    {
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
                MainUI.instance.ShowWaveUI();
            else
                MainUI.instance.ShowBossWaveUI(monsterName);
        }
        onTimerUp?.Invoke(monsterName);
    }

    [ClientRpc]
    private void NotifyClientMonsterCountClientRpc(int count)
    {
        currentMonsterCount = count;
    }
}

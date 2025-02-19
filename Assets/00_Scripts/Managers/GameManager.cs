using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;


public delegate void OnGoldGetEventHandler();
public delegate void OnTimerUpEventHandler();
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

    public int wave = -1;
    public float timer = 0;
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
    }

    private void InitGameManager()
    {
        Time.timeScale = 0f;
        if (IsServer)
        {
            waveData = Resources.LoadAll<WaveScriptable>("WaveDatas");
            InitWave();
        }
    }

    private void Update()
    {
        TimeUpdate();
    }

    private void TimeUpdate()
    {
        if (IsServer)
        {
            bool waveUI = false;
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                timer = Mathf.Max(timer, 0);
            
            }
            else {
                waveUI = true;
                InitWave();
            }
            NotifyTimerClientRpc(timer, waveData[wave].wave, waveUI);
        }
    }

    private void InitWave()
    {
        wave++;
        timer = waveData[wave].waveTime;
        Spawner.instance.StartMonsterSpawn(
            waveData[wave].spawnMonsterData.monsterName,
            waveData[wave].spawnInterval,
            waveData[wave].waveMonsterCount);
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

    [ClientRpc]
    private void GiveGoldToAllPlayersClientRpc(int gold)
    {
        this.gold += gold;
        onGoldGet?.Invoke();
    }

    [ClientRpc]
    private void NotifyTimerClientRpc(float time,int wave,bool waveUI)
    {
        this.timer = time;
        this.wave = wave;
        if(waveUI)
            MainUI.instance.ShowWaveUI();
        onTimerUp?.Invoke();
    }

    [ClientRpc]
    private void NotifyClientMonsterCountClientRpc(int count)
    {
        currentMonsterCount = count;
    }

}

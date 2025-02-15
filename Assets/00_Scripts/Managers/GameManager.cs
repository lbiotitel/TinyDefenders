using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;


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
    [field: SerializeField] public int gold { get; set; }
    [field: SerializeField] public int summonGold { get; set; }
    [field: SerializeField] public int summonGoldInc { get; set; }
    [SerializeField] private float waveTime = 60f;
    public float timer = 0;
    public int wave = 1;
    public int currentMonsterCount = 0;

    //event
    public event OnGoldGetEventHandler onGoldGet;
    public event OnTimerUpEventHandler onTimerUp;   

    private List<Monster> monsterList = new List<Monster>();


    private void Start()
    {
        timer = waveTime;
    }

    private void Update()
    {
        TimeUpdate();
    }

    private void TimeUpdate()
    {
        if (IsServer)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                timer = Mathf.Max(timer, 0);
            
            }
            else {
                wave++;
                timer = waveTime;
            }
            NotifyTimerClientRpc(timer,wave);
        }
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
    private void NotifyTimerClientRpc(float time,int wave)
    {
        this.timer = time;
        this.wave = wave;
        onTimerUp?.Invoke();
    }

    [ClientRpc]
    private void NotifyClientMonsterCountClientRpc(int count)
    {
        currentMonsterCount = count;
    }
}

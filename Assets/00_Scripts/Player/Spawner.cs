using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.DebugUI;

public class Spawner : NetworkBehaviour
{
    public static Spawner instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    [System.Serializable]
    public class SpawnNode
    {
        public Vector2 spawnPos;
        public bool isSpawned;

        public SpawnNode(Vector2 spawnPos, bool isSpawned)
        {
            this.spawnPos = spawnPos;
            this.isSpawned = isSpawned;
        }
    };

    [SerializeField] private GameObject summonEffect;
    [SerializeField] private float effectSpeed;

    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float spawnTime;

    [SerializeField] private HeroHolder heroHolderPrefab;
    [SerializeField] private Monster monsterPrefab;

    [SerializeField] private Tilemap hostTileMap;
    [SerializeField] private Tilemap clientTimeMap;

    [SerializeField] private Transform monsterSpawnPos;

    private List<Transform> ServerMonsterMovePosition = new List<Transform>();
    private List<Transform> ClientMonsterMovePosition = new List<Transform>();  

    public List<SpawnNode> hostSpawnList = new List<SpawnNode>();
    public List<SpawnNode> clientSpawnList = new List<SpawnNode>();

    public Dictionary<string, HeroHolder> heroHolders = new Dictionary<string, HeroHolder>();
    private int[] heroHolderValue = new int[2];
    HeroScriptable heroData;

    private int serverClientHolderReadyValue = 0;

    private void Start()
    {
        DrawGrid(hostTileMap, ref hostSpawnList, "HOST", 0);
        DrawGrid(clientTimeMap, ref clientSpawnList, "CLIENT", 1);

        InitMonsterSpawnPosition();
    }



    #region Initialize

    //몬스터 스폰과 이동경로 설정
    private void InitMonsterSpawnPosition()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            ServerMonsterMovePosition.Add(transform.GetChild(0).GetChild(i).transform);
        }

        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            ClientMonsterMovePosition.Add(transform.GetChild(1).GetChild(i).transform);
        }
    }


    //스프라이트의 크기에 맞게 스폰포인트 위치 계산
    private void DrawGrid(Tilemap tileMap, ref List<SpawnNode> spawnList,
        string id, int value)
    {

        BoundsInt bounds = tileMap.cellBounds;

        for (int row = bounds.yMax; row >= bounds.yMin; row--)
        {
            for (int col = bounds.xMax; col >= bounds.xMin; col--)
            {
                Vector3Int tilePosition = new Vector3Int(col,row,0);
                if (!tileMap.HasTile(tilePosition)) continue;  // 타일이 없으면 무시

                Vector3 worldPos = tileMap.CellToWorld(tilePosition);

                spawnList.Add(new SpawnNode(worldPos + new Vector3(0,0.30f,0), false));
            }
        }

        if (IsServer)
        {
            StartCoroutine(WaitForClient(id,value));
        }

        heroHolderValue[value] = 0;
    }

    private IEnumerator WaitForClient(string id,int value)
    {
        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                StartCoroutine(DelaySetHeroHolder(id, value));
            }
        }
    }

    //히어로를 가지고있을 히어로 홀더들을 서버에서 생성
    private IEnumerator DelaySetHeroHolder(string id, int value)
    {
        HeroHolder heroHolder = Instantiate(heroHolderPrefab);
        NetworkObject networkObject = heroHolder.GetComponent<NetworkObject>();
        networkObject.Spawn();

        string heroHolderId = id + heroHolderValue[value].ToString();
        heroHolderValue[value]++;

        yield return new WaitForSeconds(0.3f);

        SetHeroHolderPositionClientRpc(networkObject.NetworkObjectId, heroHolderId);
    }


    //생성된 히어로 홀더를 호스트와 클라이언트에 따라 각각 다르게 위치 처리
    [ClientRpc]
    private void SetHeroHolderPositionClientRpc(ulong networkObjectId, string heroHolderId)
    {
        if (NetUtils.TryGetSpawnedObject(networkObjectId, out NetworkObject networkObject))
        {
            List<SpawnNode> spawnList = null;
            if (heroHolderId.Contains("HOST"))
            {
                spawnList = hostSpawnList;
            }
            else if (heroHolderId.Contains("CLIENT"))
            {
                spawnList = clientSpawnList;
            }

            HeroHolder heroHolder = networkObject.GetComponent<HeroHolder>();
            SetHeroHolderPosition(networkObject, spawnList);
            heroHolders.Add(heroHolderId, heroHolder);
            heroHolder.holderId = heroHolderId;

            if (heroHolderId.Contains("CLIENT" + (gridHeight * gridWidth - 1).ToString()))
            {
                FinishSpawnHeroHolderServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void FinishSpawnHeroHolderServerRpc()
    {
        serverClientHolderReadyValue++;
        if (serverClientHolderReadyValue == 2)
        {
            NotifyStartGameClientRpc();
        }
    }

    [ClientRpc]
    private void NotifyStartGameClientRpc()
    {
        GameManager.instance.StartGame();
    }

    //히어로홀더 위치 조정
    private void SetHeroHolderPosition(NetworkObject networkObject, List<SpawnNode> spawnPositionList)
    {
        int positionIndex = -1;

        for (int i = 0; i < spawnPositionList.Count; i++)
        {
            if (spawnPositionList[i].isSpawned == false)
            {
                positionIndex = i;
                spawnPositionList[i].isSpawned = true;
                networkObject.transform.position = spawnPositionList[positionIndex].spawnPos;
                break;
            }
        }

        if (positionIndex == gridHeight * gridWidth - 1)
        {
            FinishSpawnHeroHolderServerRpc();
        }

        HeroHolder heroHolder = networkObject.GetComponent<HeroHolder>();
        heroHolder.holderIndex = positionIndex;
        heroHolder.InitCollider();
    }

    #endregion



    #region SwapHero
    public void HeroHolderSwap(string strHeroHolder1, string strHeroHolder2)
    {
        NetUtils.HostAndClientMethod(
            () => HeroHolderSwapServerRpc(strHeroHolder1, strHeroHolder2),
            () => HeroHolderSwapClient(strHeroHolder1, strHeroHolder2)
            );
    }

    [ServerRpc(RequireOwnership = false)]
    private void HeroHolderSwapServerRpc(string strHeroHolder1, string strHeroHolder2)
    {
        HeroHolderSwapClient(strHeroHolder1, strHeroHolder2);
    }

    private void HeroHolderSwapClient(string strHeroHolder1, string strHeroHolder2)
    {
        HeroHolderSwapLogicClientRpc(strHeroHolder1, strHeroHolder2);
    }

    [ClientRpc]
    private void HeroHolderSwapLogicClientRpc(string strHeroHolder1, string strHeroHolder2)
    {
        HeroHolder heroHolder1 = heroHolders[strHeroHolder1];
        HeroHolder heroHolder2 = heroHolders[strHeroHolder2];

        heroHolder1.SwapPosition(heroHolder2);
        heroHolder2.SwapPosition(heroHolder1);

        (heroHolder1.holderName, heroHolder2.holderName) = (heroHolder2.holderName, heroHolder1.holderName);
        (heroHolder1.ownHero, heroHolder2.ownHero) =
            (heroHolder2.ownHero, heroHolder1.ownHero);
        (heroHolder1.heroData, heroHolder2.heroData) = (heroHolder2.heroData, heroHolder1.heroData);
    }
    #endregion



    #region MonsterSpawn
    //몬스터 소환

    public void StartMonsterSpawn(string monsterName, float spawnInterval, int spawnCount)
    {
        StartCoroutine(SpawnMonsterCoroutine(monsterName, spawnInterval, spawnCount));
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartMonsterSpawnServerRpc(string monsterName, float spawnInterval, int spawnCount)
    {
        StartMonsterSpawn(monsterName,spawnInterval,spawnCount);
    }


    private IEnumerator SpawnMonsterCoroutine(string monsterName, float spawnInterval, int spawnCount)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            yield return new WaitForSeconds(spawnInterval);
            MonsterSpawn(true, monsterName);
            MonsterSpawn(false, monsterName);
        }
    }


    private void MonsterSpawn(bool isServer, string monsterName)
    {
        if (NetUtils.IsServer())
        {
            NetworkObject monster = NetworkPoolingManager.instance.GetObject(monsterSpawnPos.position, Quaternion.identity);

            GameManager.instance.AddMonster(monster.GetComponent<Monster>());

            MonsterSpawnClientRpc(monster.NetworkObjectId, isServer, monsterName);
        }
    }

    [ClientRpc]
    private void MonsterSpawnClientRpc(ulong networkObjectId, bool isServer, string monsterName)
    {
        if (NetUtils.TryGetSpawnedObject(networkObjectId, out NetworkObject monsterNetworkObject))
        {
            MonsterScriptable monsterData = Resources.Load<MonsterScriptable>("MonsterDatas/" + monsterName);

            List<Transform> spawnPoints = isServer ? ServerMonsterMovePosition : ClientMonsterMovePosition;

            monsterNetworkObject.transform.position = spawnPoints[0].position;

            monsterNetworkObject.GetComponent<Monster>().SetMoveTarget(spawnPoints);
            monsterNetworkObject.GetComponent<Monster>().InitData(monsterData);
        }
    }
    #endregion


    #region Hero spawn

    public HeroScriptable LoadData(string rarity)
    {
        Debug.Log(rarity);
        HeroScriptable[] heroDatas = Resources.LoadAll<HeroScriptable>("HeroDatas/" + rarity);

        HeroScriptable data = heroDatas[UnityEngine.Random.Range(0, heroDatas.Length)];

        return data;
    }

    private void HeroSpawn(ulong clientId, string rarity, string dataName)
    {

        heroData = Resources.Load<HeroScriptable>("HeroDatas/" + rarity + "/" + dataName);

        string temp = clientId == 0 ? "HOST" : "CLIENT";
        int value = clientId == 0 ? 0 : 1;
        string heroHolderId = temp + heroHolderValue[value].ToString();

        HeroHolder findHeroHolder = GetRemainHeroHolder(temp, this.heroData.heroName);

        AlarmClientRpc(clientId, this.heroData.heroName, heroData.rarity.ToString());

        if (findHeroHolder != null)
        {
            findHeroHolder.SpawnHero(this.heroData.GetHeroData(), rarity);
            return;
        }

        NetworkObject networkObject = heroHolders[heroHolderId].GetComponent<NetworkObject>();

        HeroSpawnClientRpc(networkObject.NetworkObjectId, this.heroData.GetHeroData(), rarity, value);
    }

    public Vector3 GetHolderPosition(HeroScriptable heroData)
    {
        string temp = NetUtils.GetLocalD() == 0 ? "HOST" : "CLIENT";
        int value = NetUtils.GetLocalD() == 0 ? 0 : 1;
        string heroHolderId = temp + heroHolderValue[value].ToString();

        HeroHolder findHeroHolder = GetRemainHeroHolder(temp, heroData.heroName);

        if (findHeroHolder != null)
        {
            return findHeroHolder.transform.position;
        }

        return heroHolders[heroHolderId].transform.position;
    }

    private HeroHolder GetRemainHeroHolder(string client, string heroName)
    {
        foreach (var holder in heroHolders)
        {
            if (holder.Key.Contains(client) && holder.Value.ownHero == null)
            {
                return holder.Value;
            }
        }

        return null;
    }



    [ClientRpc]
    private void HeroSpawnClientRpc(ulong networkObjectId, HeroData heroData, string rarity, int value)
    {
        if (NetUtils.TryGetSpawnedObject(networkObjectId, out NetworkObject networkObject))
        {
            HeroHolder heroHolder = networkObject.GetComponent<HeroHolder>();
            heroHolderValue[value]++;
            heroHolder.SpawnHero(heroData, rarity);
        }
    }

    [ClientRpc]
    private void AlarmClientRpc(ulong clientId, string heroName, string rarity)
    {
        if (clientId == NetUtils.GetLocalD())
        {
            InGameUI.instance.ShowAlarmText(string.Format("영웅을 획득하였습니다. {0}{1}",
                NetUtils.GetRarityColor((HeroRarity)Enum.Parse(typeof(HeroRarity), rarity)),
                heroName));
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void HeroSpawnServerRpc(ulong cleintId, string rarity, string dataName)
    {
        HeroSpawn(cleintId, rarity, dataName);
    }

    //캐릭터 소환
    public void Summon(string rarity, HeroScriptable heroScriptableData = null)
    {
        heroData = heroScriptableData;
        if (heroScriptableData == null)
            heroData = LoadData(rarity);

        NetUtils.HostAndClientMethod(
            () => HeroSpawnServerRpc(NetUtils.GetLocalD(), rarity, heroData.name),
            () => HeroSpawn(NetUtils.GetLocalD(), rarity, heroData.name)
            );
    }

    #endregion

    public void SummonHero(HeroScriptable heroData, Transform cardTransform)
    {
        StartCoroutine(SummonCoroutine(heroData, cardTransform));
    }

    private IEnumerator SummonCoroutine(HeroScriptable heroData, Transform cardTransform)
    {
        Vector3 targetPos = Spawner.instance.GetHolderPosition(heroData);

        Vector3 buttonWorldPosition = Camera.main.ScreenToWorldPoint(cardTransform.position);
        GameObject effect = Instantiate(summonEffect);

        effect.transform.position = buttonWorldPosition;

        Vector3 startPoint = buttonWorldPosition;
        Vector3 endPoint = targetPos;

        Vector3 randomPoint = MathUtils.GenerateRandomPoint(startPoint, endPoint);

        float elapsedTime = 0.0f;
        while (elapsedTime < effectSpeed)
        {
            float t = elapsedTime / effectSpeed;

            Vector3 curvePoint = MathUtils.GenerateBezierPoint(t, startPoint,
                randomPoint, endPoint);

            effect.transform.position = new Vector3(curvePoint.x, curvePoint.y, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(effect);
        Spawner.instance.Summon(heroData.rarity.ToString(), heroData);
    }
}

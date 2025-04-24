using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.DebugUI;

public class SpawnManager : NetworkBehaviour
{
    public static SpawnManager instance = null;
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

    [SerializeField] private HeroHolder heroHolderPrefab;
    [SerializeField] private Monster monsterPrefab;

    [SerializeField] private Tilemap hostTileMap;
    [SerializeField] private Tilemap clientTimeMap;

    [SerializeField] private Transform monsterSpawnPos;

    [SerializeField] private string summonHeroSFX;

    private List<Transform> ServerMonsterMovePosition = new List<Transform>();
    private List<Transform> ClientMonsterMovePosition = new List<Transform>();

    private List<SpawnNode> hostSpawnPositionList = new List<SpawnNode>();
    private List<SpawnNode> clientSpawnPositionList = new List<SpawnNode>();

    private List<Hero> clientHeroList = new List<Hero>();
    private List<Hero> serverHeroList = new List<Hero>();

    private List<Monster> monsterList = new List<Monster>();


    public Dictionary<string, HeroHolder> heroHolders = new Dictionary<string, HeroHolder>();
    private int[] heroHolderValue = new int[2];

    private int serverClientHolderReadyValue = 0;

    public void SetSpawnManager()
    {
        DrawGrid(hostTileMap, ref hostSpawnPositionList, "HOST", 0);
        DrawGrid(clientTimeMap, ref clientSpawnPositionList, "CLIENT", 1);

        InitMonsterSpawnPosition();

        EventManager.instance.onWaveNotClearAction += ClearMonster;
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
        
        float cellWidth = tileMap.cellSize.x / 2f;
        float cellHeight = tileMap.cellSize.y / 2f;

        for (int row = bounds.yMax; row >= bounds.yMin; row--)
        {
            for (int col = bounds.xMin; col <= bounds.xMax; col++)
            {
                Vector3Int tilePosition = new Vector3Int(col, row, 0);
                if (!tileMap.HasTile(tilePosition)) continue;  

                Vector3 worldPos = tileMap.CellToWorld(tilePosition);

                spawnList.Add(new SpawnNode(worldPos + new Vector3(cellWidth,cellHeight,0), false));
            }
        }

        int maxHeroCount = spawnList.Count;
        PlayerStatManager.instance.maxHeroCount = maxHeroCount;

        if (id == "CLIENT") spawnList.Reverse();

        SetHeroHolders(maxHeroCount, id, value);

        heroHolderValue[value] = 0;
    }

    private void SetHeroHolders(int count, string id, int value)
    {
        if (NetUtils.IsHost())
        {
            for (int i = 0; i < count; i++)
            {
                StartCoroutine(DelaySetHeroHolder(id, value,count));
            }
        }
    }


    //히어로를 가지고있을 히어로 홀더들을 서버에서 생성
    private IEnumerator DelaySetHeroHolder(string id, int value,int count)
    {
        HeroHolder heroHolder = Instantiate(heroHolderPrefab);
        NetworkObject networkObject = heroHolder.GetComponent<NetworkObject>();
        networkObject.Spawn();

        string heroHolderId = id + heroHolderValue[value].ToString();
        heroHolderValue[value]++;

        yield return new WaitForSeconds(0.3f);

        SetHeroHolderPositionClientRpc(networkObject.NetworkObjectId, heroHolderId,count);
    }


    //생성된 히어로 홀더를 호스트와 클라이언트에 따라 각각 다르게 위치 처리
    [ClientRpc]
    private void SetHeroHolderPositionClientRpc(ulong networkObjectId, string heroHolderId,int count)
    {
        if (NetUtils.TryGetSpawnedObject(networkObjectId, out NetworkObject networkObject))
        {
            List<SpawnNode> spawnList = null;
            if (heroHolderId.Contains("HOST"))
            {
                spawnList = hostSpawnPositionList;
            }
            else if (heroHolderId.Contains("CLIENT"))
            {
                spawnList = clientSpawnPositionList;
            }

            HeroHolder heroHolder = networkObject.GetComponent<HeroHolder>();
            SetHeroHolderPosition(networkObject, spawnList);
            heroHolders.Add(heroHolderId, heroHolder);
            heroHolder.holderId = heroHolderId;

            if (heroHolderId.Contains("CLIENT" + (count - 1).ToString()))
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
        StopAllCoroutines();
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
            MonsterSpawn(true, monsterName);
            MonsterSpawn(false, monsterName);
            yield return new WaitForSeconds(spawnInterval);
        }
    }


    private void MonsterSpawn(bool isServer, string monsterName)
    {
        if (NetUtils.IsHost())
        {
            NetworkObject monster = NetworkPoolingManager.instance.GetObject(monsterSpawnPos.position, Quaternion.identity);

            AddMonster(monster.GetComponent<Monster>());

            MonsterSpawnClientRpc(monster.NetworkObjectId, isServer, monsterName);
        }
    }

    [ClientRpc]
    private void MonsterSpawnClientRpc(ulong networkObjectId, bool isServer, string monsterName)
    {
        if (NetUtils.TryGetSpawnedObject(networkObjectId, out NetworkObject monsterNetworkObject))
        {
            MonsterScriptable monsterData = GameResourceManager.instance.GetMonsterData(monsterName);

            List<Transform> movePoints = isServer ? ServerMonsterMovePosition : ClientMonsterMovePosition;

            monsterNetworkObject.transform.position = movePoints[0].position;

            monsterNetworkObject.GetComponent<NetworkTransform>().Interpolate = true;
            monsterNetworkObject.GetComponent<Monster>().InitData(monsterData,movePoints,isServer);
        }
    }

    public int GetAliveMonsterCount() 
    {
        int count = 0;
        monsterList.ForEach(m => count += m.IsDead() ? 0 : 1);
        return count;
    
    }

    private void ClearMonster()
    {
        foreach (var monster in monsterList)
        {
            monster.DeadClientRpc();
        }
    }

    public void AddMonster(Monster monster)
    {
        monsterList.Add(monster);
    }
    public void RemoveMonster(Monster monster)
    {
        monsterList.Remove(monster);
    }




    #endregion


    #region Hero spawn


    private void HeroSpawn(PlayerNetworkType playerNetworkType, string heroName, int heroLevel = 1)
    {
        int value = playerNetworkType == PlayerNetworkType.HOST ? 0 : 1;
        string heroHolderId = playerNetworkType + heroHolderValue[value].ToString();

        HeroHolder findHeroHolder = GetRemainHeroHolder(playerNetworkType);

        if (findHeroHolder != null)
        {
            findHeroHolder.SpawnHero(heroName,heroLevel);
            return;
        }
    }

    public Vector3 GetHolderPosition()
    {
        PlayerNetworkType playerNetworkType = NetUtils.GetPlayerNetworkType();
        int value = playerNetworkType == PlayerNetworkType.HOST ? 0 : 1;
        string heroHolderId = playerNetworkType + heroHolderValue[value].ToString();

        HeroHolder findHeroHolder = GetRemainHeroHolder(playerNetworkType);

        if (findHeroHolder != null)
        {
            return findHeroHolder.transform.position;
        }

        return Vector3.zero;
    }

    private HeroHolder GetRemainHeroHolder(PlayerNetworkType playerNetworkType)
    {
        foreach (var holder in heroHolders)
        {
            if (holder.Key.Contains(playerNetworkType.ToString()) && holder.Value.ownHero == null)
            {
                return holder.Value;
            }
        }

        return null;
    }



    [ClientRpc]
    private void HeroSpawnClientRpc(ulong networkObjectId, string heroName,int heroLevel ,int value)
    {
        if (NetUtils.TryGetSpawnedObject(networkObjectId, out NetworkObject networkObject))
        {
            HeroHolder heroHolder = networkObject.GetComponent<HeroHolder>();
            heroHolderValue[value]++;
            heroHolder.SpawnHero(heroName, heroLevel);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void HeroSpawnServerRpc(PlayerNetworkType playerNetworkType, string dataName,int heroLevel = 1)
    {
        HeroSpawn(playerNetworkType, dataName, heroLevel);
    }

    //캐릭터 소환
    public void Spawn(string heroName,int heroLevel = 1)
    {
        SoundManager.instance.PlaySfxSound(summonHeroSFX, SFXTYPE.EXTRA);

        NetUtils.HostAndClientMethod(
            () => HeroSpawnServerRpc(NetUtils.GetPlayerNetworkType(), heroName,heroLevel),
            () => HeroSpawn(NetUtils.GetPlayerNetworkType(), heroName, heroLevel)
            );
    }

    #endregion

    public void Spawn(string heroName, Transform cardTransform)
    {
        StartCoroutine(SummonCoroutine(heroName, cardTransform));
    }

    private IEnumerator SummonCoroutine(string heroName, Transform cardTransform)
    {
        Vector3 targetPos = GetHolderPosition();

        Vector3 cardWorldPosition = Camera.main.ScreenToWorldPoint(cardTransform.position);
        GameObject effect = Instantiate(summonEffect);

        effect.transform.position = cardWorldPosition;

        Vector3 startPoint = cardWorldPosition;
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
        Spawn(heroName);
    }




    #region About Hero Management
    public void AddHero(Hero hero)
    {
        PlayerNetworkType playerNetworkType = hero.heroOwnerNetworkType;

        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverHeroList.Add(hero);
                break;
            case PlayerNetworkType.CLIENT:
                clientHeroList.Add(hero);
                break;
        }

        EventManager.instance.onHeroSpawnAction?.Invoke(hero);
    }

    public void RemoveHero(Hero hero)
    {
        PlayerNetworkType playerNetworkType = hero.heroOwnerNetworkType;

        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                serverHeroList.Remove(hero);
                break;
            case PlayerNetworkType.CLIENT:
                clientHeroList.Remove(hero);
                break;
        }

        EventManager.instance.onHeroDespawnAction?.Invoke(hero);
    }

    public bool CanSpawn(PlayerNetworkType playerNetworkType)
    {
        int currentHeroCount = GetHeroList(playerNetworkType).Count;

        if (currentHeroCount >= PlayerStatManager.instance.currentMaxHeroCount)
        {
            AdviceUIManager.instance.AddAdvice("인원수가 꽉 찼습니다.");
            return false;
        }
        return true;
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


    public int GetHeroCount(PlayerNetworkType playerNetworkType)
    {
        return GetHeroList(playerNetworkType).Count;
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
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

    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float spawnTime;

    [SerializeField] private HeroHolder heroHolderPrefab;
    [SerializeField] private Monster monsterPrefab;
    [SerializeField] private GameObject test;

    private List<Transform> playerMonsterMoveList = new List<Transform>();
    private List<Transform> friendMonsterMoveList = new List<Transform>();

    private static List<SpawnNode> playerSpawnList = new List<SpawnNode>();
    private static List<SpawnNode> friendSpawnList = new List<SpawnNode>();

    private Dictionary<string, HeroHolder> heroHolders = new Dictionary<string, HeroHolder>();

    public static float xValue, yValue;

    private void Start()
    {
        DrawGrid(transform.GetChild(1), true);
        DrawGrid(transform.GetChild(0), false);
        InitMonsterSpawnPosition();
        StartCoroutine(SpawnMonsterCoroutine());
    }


    private void InitMonsterSpawnPosition()
    {
        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            playerMonsterMoveList.Add(transform.GetChild(0).GetChild(i).transform);
        }

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            friendMonsterMoveList.Add(transform.GetChild(1).GetChild(i).transform);
        }
    }

    private void DrawGrid(Transform targetTransform, bool player)
    {
        SpriteRenderer spriteRenderer = targetTransform.GetComponent<SpriteRenderer>();
        float width = spriteRenderer.bounds.size.x;
        float height = spriteRenderer.bounds.size.y;

        float xCount = width / gridWidth;
        float yCount = height / gridHeight;

        xValue = xCount;
        yValue = yCount;

        Vector2 startPos = new Vector2(targetTransform.position.x - width / 2f,
                                       targetTransform.position.y - height / 2f);

        for (int row = 0; row < gridHeight; row++)
        {
            for (int col = 0; col < gridWidth; col++)
            {
                float xPos = startPos.x + (col * xCount) + (xCount / 2f);
                float yPos;

                if (player)
                {
                    yPos = targetTransform.position.y + height / 2f - (row * yCount) - (yCount / 2f);
                    playerSpawnList.Add(new SpawnNode(new Vector2(xPos, yPos), false));
                }
                else
                {
                    yPos = targetTransform.position.y - height / 2f + (row * yCount) + (yCount / 2f);
                    friendSpawnList.Add(new SpawnNode(new Vector2(xPos, yPos), false));
                }
            }
        }
    }

    public static void HeroHolderSwap(HeroHolder heroHolder1, HeroHolder heroHolder2)
    {
        heroHolder1.SwapPosition(heroHolder2);
        heroHolder2.SwapPosition(heroHolder1);

        List<Hero> heroList1 = new List<Hero>(heroHolder1.heroList);
        List<Hero> heroList2 = new List<Hero>(heroHolder2.heroList);

        heroHolder1.heroList = new List<Hero>(heroList2);
        heroHolder2.heroList = new List<Hero>(heroList1);
    }



    #region MonsterSpawn
    //몬스터 소환
    IEnumerator SpawnMonsterCoroutine()
    {
        yield return new WaitForSeconds(spawnTime);

        if (IsClient)
        {
            MonsterSpawnServerRpc(GetLocalId());
        }
        else if (IsServer)
        {
            MonsterSpawn(GetLocalId());
        }

        StartCoroutine(SpawnMonsterCoroutine());
    }

    [ServerRpc(RequireOwnership = false)]
    private void MonsterSpawnServerRpc(ulong clientId)
    {
        MonsterSpawn(clientId);
    }

    private void MonsterSpawn(ulong clientId)
    {
        Monster go = Instantiate(monsterPrefab);
        NetworkObject networkObject = go.GetComponent<NetworkObject>();
        networkObject.Spawn();
        GameManager.instance.AddMonster(go);

        MonsterSpawnClientRpc(networkObject.NetworkObjectId, clientId);
    }

    [ClientRpc]
    private void MonsterSpawnClientRpc(ulong networkObjectId, ulong clientId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.
            TryGetValue(networkObjectId, out NetworkObject monsterNetworkObject))
        {
            if (clientId == GetLocalId())
            {
                monsterNetworkObject.transform.position = playerMonsterMoveList[0].position;
                monsterNetworkObject.GetComponent<Monster>().SetMoveTarget(playerMonsterMoveList);
            }
            else
            {
                monsterNetworkObject.transform.position = friendMonsterMoveList[0].position;
                monsterNetworkObject.GetComponent<Monster>().SetMoveTarget(friendMonsterMoveList);
            }
        }
    }
    #endregion


    #region Hero spawn
    private void HeroSpawn(ulong clientId)
    {
        HeroScriptable[] heroDatas = Resources.LoadAll<HeroScriptable>("HeroDatas");

        HeroScriptable data = heroDatas[UnityEngine.Random.Range(0, heroDatas.Length)];

        bool getHero = false;

        string temp = clientId == 0 ? "HOST" : "CLIENT";

        foreach (var dd in heroHolders)
        {
            if (dd.Key.Contains(temp))
            {
                if (dd.Value.heroList.Count < 3 && dd.Value.holderName == data.heroName)
                {
                    dd.Value.SpawnHero(data.GetHeroData());
                    getHero = true;
                    break;
                }
            }
        }

        if (!getHero)
        {
            HeroHolder heroHolder = Instantiate(heroHolderPrefab);
            NetworkObject networkObject = heroHolder.GetComponent<NetworkObject>();
            networkObject.Spawn();
       
            heroHolders.Add(temp + heroHolders.Count.ToString(), heroHolder);

            UpdateHeroSpawnPositionClientRpc(networkObject.NetworkObjectId, clientId, data.GetHeroData());
        }
    }


    [ClientRpc]
    private void UpdateHeroSpawnPositionClientRpc(ulong networkObjectId, ulong clientId, HeroData heroData)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject networkObject))
        {
            if (clientId == GetLocalId())
            {
                SetHeroPosition(networkObject, ref playerSpawnList);
            }
            else
            {
                SetHeroPosition(networkObject, ref friendSpawnList);
            }
            networkObject.GetComponent<HeroHolder>().SpawnHero(heroData);
        }
    }

    private void SetHeroPosition(NetworkObject networkObject, ref List<SpawnNode> spawnPositionList)
    {
        int positionIndex = -1;

        for (int i = 0; i < spawnPositionList.Count; i++)
        {
            if (spawnPositionList[i].isSpawned == false)
            {
                positionIndex = i;
                spawnPositionList[i].isSpawned = true;
                break;
            }
        }

        HeroHolder heroHolder = networkObject.GetComponent<HeroHolder>();
        heroHolder.transform.position = spawnPositionList[positionIndex].spawnPos;
        heroHolder.index = positionIndex;
    }

    [ServerRpc(RequireOwnership = false)]
    private void HeroSpawnServerRpc(ulong cleintId)
    {
        HeroSpawn(cleintId);
    }

    //캐릭터 소환
    public void Summon()
    {
        /*if (GameManager.instance.gold < GameManager.instance.summonGold)
        {
            return;
        }

        GameManager.instance.gold -= GameManager.instance.summonGold;
        GameManager.instance.summonGold += GameManager.instance.summonGoldInc;*/

        if (IsClient)
        {
            HeroSpawnServerRpc(GetLocalId());
        }
        else if (IsServer)
        {
            HeroSpawn(GetLocalId());
        }
    }

    #endregion

    private ulong GetLocalId()
    {
        return NetworkManager.Singleton.LocalClientId;
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
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

    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float spawnTime;

    [SerializeField] private HeroHolder heroHolderPrefab;
    [SerializeField] private Monster monsterPrefab;
    [SerializeField] private GameObject test;

    private List<Transform> playerMonsterMoveList = new List<Transform>();
    private List<Transform> friendMonsterMoveList = new List<Transform>();

    public List<SpawnNode> playerSpawnList = new List<SpawnNode>();
    public List<SpawnNode> friendSpawnList = new List<SpawnNode>();

    public Dictionary<string, HeroHolder> heroHolders = new Dictionary<string, HeroHolder>();
    private int[] heroHolderValue = new int[2];

    public float xValue, yValue;

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
                                       targetTransform.position.y + height / 2f);

        for (int row = 0; row < gridHeight; row++)
        {
            for (int col = 0; col < gridWidth; col++)
            {
                float xPos;
                float yPos = startPos.y - (row * yCount) - (yCount / 2f);

                if (player)
                {
                    xPos = targetTransform.position.x + width / 2f - (col * xCount) - (xCount / 2f);
                    playerSpawnList.Add(new SpawnNode(new Vector2(xPos, yPos), false));
                }
                else
                {
                    xPos = targetTransform.position.x - width / 2f + (col * xCount) + (xCount / 2f);
                    friendSpawnList.Add(new SpawnNode(new Vector2(xPos, yPos), false));
                }
            }
        }
    }

    public void HeroHolderSwap(string strHeroHolder1, string strHeroHolder2)
    {
        if (IsServer)
        {
            HeroHolderSwapClient(strHeroHolder1, strHeroHolder2);
        }
        else if (IsClient)
        {
            HeroHolderSwapServerRpc(strHeroHolder1, strHeroHolder2);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void HeroHolderSwapServerRpc(string strHeroHolder1, string strHeroHolder2)
    {
        HeroHolderSwapClient(strHeroHolder1, strHeroHolder2);
    }

    private void HeroHolderSwapClient(string strHeroHolder1, string strHeroHolder2)
    {
        HeroHolderSwapLogicClientRpc(strHeroHolder1,strHeroHolder2);
    }

    [ClientRpc]
    private void HeroHolderSwapLogicClientRpc(string strHeroHolder1, string strHeroHolder2)
    {
        HeroHolder heroHolder1 = heroHolders[strHeroHolder1];
        HeroHolder heroHolder2 = heroHolders[strHeroHolder2];   

        heroHolder1.SwapPosition(heroHolder2);
        heroHolder2.SwapPosition(heroHolder1);

        List<Hero> heroList1 = new List<Hero>(heroHolder1.heroList);
        List<Hero> heroList2 = new List<Hero>(heroHolder2.heroList);

        heroHolder1.heroList = new List<Hero>(heroList2);
        heroHolder2.heroList = new List<Hero>(heroList1);

        HeroData heroData1 = heroHolder1.heroData;
        HeroData heroData2 = heroHolder2.heroData;

        heroHolder1.heroData = heroData2;
        heroHolder2.heroData = heroData1;
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
    private void HeroSpawn(ulong clientId,string rarity)
    {
        HeroScriptable[] heroDatas = Resources.LoadAll<HeroScriptable>("HeroDatas/" + rarity);

        HeroScriptable data = heroDatas[UnityEngine.Random.Range(0, heroDatas.Length)];

        bool getHero = false;

        string temp = clientId == 0 ? "HOST" : "CLIENT";
        int value = clientId == 0 ? 0 : 1;
        string heroHolderId = temp + heroHolderValue[value].ToString();

        foreach (var dd in heroHolders)
        {
            if (dd.Key.Contains(temp))
            {
                if (dd.Value.heroList.Count < 3 && dd.Value.holderName == data.heroName)
                {
                    dd.Value.SpawnHero(data.GetHeroData(),rarity);
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

            UpdateHeroSpawnPositionClientRpc(networkObject.NetworkObjectId, clientId, data.GetHeroData(),heroHolderId,value,rarity);
        }
    }


    [ClientRpc]
    private void UpdateHeroSpawnPositionClientRpc(ulong networkObjectId, ulong clientId, HeroData heroData,string heroHolderId,int value,
        string rarity)
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

            HeroHolder heroHolder = networkObject.GetComponent<HeroHolder>();

            heroHolders.Add(heroHolderId, heroHolder);
            heroHolderValue[value]++;
            heroHolder.holderId = heroHolderId;

            heroHolder.SpawnHero(heroData,rarity);
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
        heroHolder.holderIndex = positionIndex;
    }

    [ServerRpc(RequireOwnership = false)]
    private void HeroSpawnServerRpc(ulong cleintId,string rarity)
    {
        HeroSpawn(cleintId,rarity);
    }

    //캐릭터 소환
    public void Summon(string rarity)
    {
        /*if (GameManager.instance.gold < GameManager.instance.summonGold)
        {
            return;
        }

        GameManager.instance.gold -= GameManager.instance.summonGold;
        GameManager.instance.summonGold += GameManager.instance.summonGoldInc;*/

        if (IsClient)
        {
            HeroSpawnServerRpc(GetLocalId(),rarity);
        }
        else if (IsServer)
        {
            HeroSpawn(GetLocalId(), rarity);
        }
    }

    #endregion

    private ulong GetLocalId()
    {
        return NetworkManager.Singleton.LocalClientId;
    }

}

using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private Monster monsterPrefab;


    //둘다 나중에 private 으로
    public List<Transform> playerMonsterMoveList = new List<Transform>();
    public List<Transform> friendMonsterMoveList = new List<Transform>();

    public List<SpawnNode> playerSpawnList = new List<SpawnNode>();
    public List<SpawnNode> friendSpawnList = new List<SpawnNode>();

    private void Start()
    {
        DrawGrid(transform.GetChild(1),ref playerSpawnList);
        DrawGrid(transform.GetChild(0),ref friendSpawnList);
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

    private void DrawGrid(Transform targetTransform,ref List<SpawnNode> spawnList)
    { 
        SpriteRenderer spriteRenderer = targetTransform.GetComponent<SpriteRenderer>();
        float width = spriteRenderer.bounds.size.x;
        float height = spriteRenderer.bounds.size.y;

        float xCount = targetTransform.localScale.x / gridWidth;
        float yCount = targetTransform.localScale.y / gridHeight;

        for (int row = 0; row < gridHeight; row++)
        { 
            for(int col = 0; col < gridWidth; col++)
            {
                float xPos = (-width / 2) + (col * xCount) + (xCount / 2);
                float yPos = (height / 2) - (row * yCount) + (yCount / 2);

                spawnList.Add(new SpawnNode(new Vector2(xPos, yPos + targetTransform.localPosition.y - yCount),false));
            }
        }
    }

    //몬스터 소환
    IEnumerator SpawnMonsterCoroutine()
    {
        yield return new WaitForSeconds(spawnTime);

        if (IsClient)
        {
            MonsterSpawnServerRpc(NetworkManager.Singleton.LocalClientId);
        }else if(IsServer)
        {
            MonsterSpawn(NetworkManager.Singleton.LocalClientId);
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
        MonsterSpawnClientRpc(networkObject.NetworkObjectId,clientId);
    }

    [ClientRpc]
    private void MonsterSpawnClientRpc(ulong networkObjectId,ulong clientId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.
            TryGetValue(networkObjectId, out NetworkObject monsterNetworkObject))
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
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


    //캐릭터 소환
    public void Summon()
    {
        if (GameManager.instance.gold < GameManager.instance.summonGold)
        {
            return;
        }

        GameManager.instance.gold -= GameManager.instance.summonGold;
        GameManager.instance.summonGold += GameManager.instance.summonGoldInc;

        int positionIndex = -1;
        GameObject go = Instantiate(knightPrefab);

        for (int i = 0; i < playerSpawnList.Count; i++)
        {
            if (playerSpawnList[i].isSpawned == false)
            {
                positionIndex = i;
                break;
            }
        }

        go.transform.position = playerSpawnList[positionIndex].spawnPos + new Vector2(0,0.1f);
        playerSpawnList[positionIndex].isSpawned = true;
    }
}

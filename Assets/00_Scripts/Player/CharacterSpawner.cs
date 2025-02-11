using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    class SpawnNode
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

    public List<Transform> monsterSpawnPositionList = new List<Transform>();
    List<SpawnNode> spawnList = new List<SpawnNode>();

    private void Start()
    {
        DrawGrid();
        InitMonsterSpawnPosition();
        StartCoroutine(SpawnMonsterCoroutine());
    }

    private void InitMonsterSpawnPosition()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            monsterSpawnPositionList.Add(transform.GetChild(i).transform);
        }
    }

    private void DrawGrid()
    { 
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float width = spriteRenderer.bounds.size.x;
        float height = spriteRenderer.bounds.size.y;

        float xCount = transform.localScale.x / gridWidth;
        float yCount = transform.localScale.y / gridHeight;

        for (int row = 0; row < gridHeight; row++)
        { 
            for(int col = 0; col < gridWidth; col++)
            {
                float xPos = (-width / 2) + (col * xCount) + (xCount / 2);
                float yPos = (height / 2) - (row * yCount) + (yCount / 2);

                spawnList.Add(new SpawnNode(new Vector2(xPos, yPos + transform.localPosition.y - yCount),false));
            }
        }
    }

    //몬스터 소환
    IEnumerator SpawnMonsterCoroutine()
    {
        Monster go = Instantiate(monsterPrefab, monsterSpawnPositionList[0].position,Quaternion.identity);
        go.SetMoveTarget(monsterSpawnPositionList);
        GameManager.instance.AddMonster(go);
        yield return new WaitForSeconds(spawnTime);
        StartCoroutine(SpawnMonsterCoroutine());
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

        for (int i = 0; i < spawnList.Count; i++)
        {
            if (spawnList[i].isSpawned == false)
            {
                positionIndex = i;
                break;
            }
        }

        go.transform.position = spawnList[positionIndex].spawnPos + new Vector2(0,0.1f);
        spawnList[positionIndex].isSpawned = true;
    }
}

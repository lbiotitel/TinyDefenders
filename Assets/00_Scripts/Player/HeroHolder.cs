using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class HeroHolder : NetworkBehaviour
{
    [SerializeField] private Hero heroPrefab;
    [SerializeField] private Transform heroRange;
    [SerializeField] private Transform heroSquare;
    [SerializeField] private Transform heroCircle;

    public string holderName;
    public string holderId;
    public List<Hero> heroList = new List<Hero>();
    public HeroData heroData;

    private readonly Vector2[] oneHeroOffset = { Vector2.zero };
    private readonly Vector2[] twoHeroOffset =
    {
        new Vector2(-0.084f,0.057f),
        new Vector2(0.135f,-0.192f)
    };
    private readonly Vector2[] threeHeroOffset =
    {
        new Vector2(-0.004f,0.113f),
        new Vector2(0.182f,-0.086f),
        new Vector2(-0.074f,-0.15f)
    };

    private void Start()
    {
        InitCollider();
        HideRange();
        HideCircle();
        HideSquare();
    }

    private void InitCollider()
    {
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(Spawner.instance.xValue, Spawner.instance.yValue);
        heroSquare.GetComponent<SpriteRenderer>().size = collider.size;
    }


    public void ShowCircle() { heroCircle.gameObject.SetActive(true); }
    public void ShowSquare() { heroSquare.gameObject.SetActive(true); } 
    public void HideCircle() { heroCircle.gameObject.SetActive(false); }
    public void HideSquare() { heroSquare.gameObject.SetActive(false); }


    public void ShowRange()
    {
        float range = heroData.heroAtkRange;
        heroRange.localScale = Vector2.one * range * 2;

        heroRange.gameObject.SetActive(true);
    }

    public void HideRange()
    {
        heroRange.gameObject.SetActive(false);
        heroRange.localScale = Vector2.zero;
    }

    public void SpawnHero(HeroData heroData)
    {
        if (heroList.Count == 0)
        {
            holderName = heroData.heroName;
            this.heroData = heroData;
        }
        if (IsServer)
        {
            HeroSpawn(GetLocalId(), heroData);
        }
    }

    public void SwapPosition(HeroHolder newHeroHolder)
    {
        Vector2[] offset = GetHeroOffset();

        Vector2[] pos = new Vector2[offset.Length];

        for (int i = 0; i < pos.Length; i++)
        {
            Vector2 worldPosition = newHeroHolder.transform.TransformPoint(offset[i]);
            pos[i] = worldPosition;
        }

        for (int i = 0; i < heroList.Count; i++)
        {
            heroList[i].SwapPosition(newHeroHolder,pos,i);
        }
    }

    private void HeroSpawn(ulong clientId,HeroData data)
    {
        Hero hero = Instantiate(heroPrefab);
        NetworkObject networkObject = hero.GetComponent<NetworkObject>();
        networkObject.Spawn();

        hero.transform.parent = this.transform;

        UpdateHeroSpawnPositionClientRpc(networkObject.NetworkObjectId,clientId, data);
    }

    private void ClientSetPosition()
    {
        for (int i = 0; i < heroList.Count; i++)
        {
            heroList[i].transform.localPosition = GetHeroOffset()[i];
            heroList[i].SortingOrderChange(i + 1);
        }
    }

    private Vector2[] GetHeroOffset()
    {
        if (heroList.Count == 1) return oneHeroOffset;
        else if (heroList.Count == 2) return twoHeroOffset;
        else if (heroList.Count == 3) return threeHeroOffset;

        return null;
    }
   

    [ClientRpc]
    private void UpdateHeroSpawnPositionClientRpc(ulong networkObjectId, ulong clientId, HeroData heroData)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject networkObject))
        {
            Hero hero = networkObject.GetComponent<Hero>();

            heroList.Add(hero);
            networkObject.GetComponent<Hero>().InitHeroData(heroData,this);
            ClientSetPosition();
        }
    }

    private ulong GetLocalId()
    {
        return NetworkManager.Singleton.LocalClientId;
    }

}

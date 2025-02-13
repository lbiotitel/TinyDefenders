using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class HeroHolder : NetworkBehaviour
{
    [SerializeField] private Hero heroPrefab;


    public string holderName;
    public List<Hero> heroList = new List<Hero>();

    private readonly Vector2[] oneHeroOffset = { Vector2.zero };
    private readonly Vector2[] twoHeroOffset = 
    {
        new Vector2(-0.08f,0.015f),
        new Vector2(0.1f,-0.161f)
    };
    private readonly Vector2[] threeHeroOffset =
    {
        new Vector2(0.05f,0.003f),
        new Vector2(0.104f,-0.195f),
        new Vector2(-0.082f,-0.133f)
    };

    private void Start()
    {
        InitCollider();
    }

    private void InitCollider()
    {
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(Spawner.xValue, Spawner.yValue);
    }


    public void SpawnHero(HeroData heroData)
    {
        if (heroList.Count == 0)
        {
            holderName = heroData.heroName;
        }
        if (IsServer)
        {
            HeroSpawn(GetLocalId(), heroData);
        }
    }

    private void HeroSpawn(ulong clientId,HeroData data)
    {
        Hero hero = Instantiate(heroPrefab);
        NetworkObject networkObject = hero.GetComponent<NetworkObject>();
        networkObject.Spawn();

        hero.transform.SetParent(transform);

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
            networkObject.GetComponent<Hero>().InitHeroData(heroData);
            ClientSetPosition();
        }
    }

    private ulong GetLocalId()
    {
        return NetworkManager.Singleton.LocalClientId;
    }

}

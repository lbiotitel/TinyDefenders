using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class HeroHolder : NetworkBehaviour
{
    [SerializeField] private Hero heroPrefab;
    public string holderName;
    public List<Hero> heroList = new List<Hero>();

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
            heroList[i].transform.localPosition = Vector2.zero;
        }
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

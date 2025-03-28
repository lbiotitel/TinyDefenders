using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeroHolder : NetworkBehaviour
{
    [SerializeField] private Hero heroPrefab;
    [SerializeField] private Transform heroRange;
    [SerializeField] private Transform heroTargetMark;
    [SerializeField] private Transform heroOwnMark;

    [SerializeField] private Color[] circleColors;
    [SerializeField] private SpriteRenderer circleSpriteRenderer;

    [SerializeField] private float circleRadius;    

    [HideInInspector]public string holderName;
    [HideInInspector] public string holderId;
    [HideInInspector] public Hero ownHero = null;
    [HideInInspector] public HeroScriptable heroData;
    [HideInInspector] public int holderIndex;
    private NetworkObjectType networkObjectType;


    private void Start()
    {
        HideHeroRange();
        HideOwnMark();
        HideHeroMoveTargetMark();
    }

    public void InitCollider()
    {
        CircleCollider2D collider = transform.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = circleRadius;
        heroTargetMark.GetComponent<SpriteRenderer>().size = new Vector2(circleRadius, circleRadius);
    }


    public void ShowOwnMark() { heroOwnMark.gameObject.SetActive(true); }
    public void HideOwnMark() { heroOwnMark.gameObject.SetActive(false); }
    public void ShowHeroMoveTargetMark() { heroTargetMark.gameObject.SetActive(true); }
    public void HideHeroMoveTargetMark() { heroTargetMark.gameObject.SetActive(false); }


    public void ShowHeroRange()
    {
        float range = heroData.atkRange.GetValue();
        heroRange.localScale = Vector2.one * range;

        heroRange.gameObject.SetActive(true);
    }

    public void HideHeroRange()
    {
        heroRange.gameObject.SetActive(false);
        heroRange.localScale = Vector2.zero;
    }

    public void SpawnHero(string heroName, string rarity)
    {
        if (IsServer)
        {
            HeroSpawn(NetUtils.GetLocalD(), heroName, rarity);
        }
    }


    #region About Sell Hero
    public void Sell()
    {
        NetUtils.HostAndClientMethod(
            () => SellServerRpc(NetUtils.GetLocalD()),
            () => SellHero(NetUtils.GetLocalD())
         );
    }

    [ServerRpc(RequireOwnership = false)]
    private void SellServerRpc(ulong clientId)
    {
        SellHero(clientId);
    }

    private void SellHero(ulong clientId)
    {
        if (ownHero == null) return;
        ulong heroId = ownHero.NetworkObjectId;
        NetworkObject obj = NetworkManager.SpawnManager.SpawnedObjects[heroId];
        SellClientRpc(heroId, clientId);
        obj.Despawn();
    }

    [ClientRpc]
    private void SellClientRpc(ulong heroKey, ulong clientId)
    {
        NetworkObject obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[heroKey];
        ownHero = null;
        if (ownHero == null)
        {
            DespawnHolderServerRpc(clientId);
        }
        ClientSetPosition();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnHolderServerRpc(ulong clientId)
    {
        DespawnHolderClientRpc(clientId,NetworkObjectId);
        holderName = string.Empty;
        ownHero = null;
    }

    [ClientRpc]
    private void DespawnHolderClientRpc(ulong clientId,ulong networkObjectId)
    {
        if (NetUtils.TryGetSpawnedObject(networkObjectId, out NetworkObject heroHolder))
        {
            HeroHolder holder = heroHolder.GetComponent<HeroHolder>();
            holder.holderName = string.Empty;
            ownHero = null;
            HideOwnMark();
            HideHeroRange();
            HideHeroMoveTargetMark();
        }
    }

    #endregion


    public void SwapPosition(HeroHolder newHeroHolder)
    {
        if (ownHero == null) return;

        ownHero.SwapPosition(newHeroHolder, newHeroHolder.transform.TransformPoint(Vector2.zero));
    }

    private void HeroSpawn(ulong clientId, string heroName, string rarity)
    {
        Hero hero = Instantiate(heroPrefab);
        

        NetworkObject networkObject = hero.GetComponent<NetworkObject>();
        networkObject.Spawn();
        hero.transform.parent = this.transform;

        UpdateHeroSpawnPositionClientRpc(networkObject.NetworkObjectId, clientId, heroName, rarity);
    }



    private void ClientSetPosition()
    {
        if (ownHero == null)
        {
            circleSpriteRenderer.color = circleColors[(int)HeroRarity.Normal];
        return;
        }
        ownHero.transform.localPosition = Vector2.zero;
        circleSpriteRenderer.color = circleColors[(int)ownHero.GetHeroData().rarity];
    }


    [ClientRpc]
    private void UpdateHeroSpawnPositionClientRpc(ulong networkObjectId, ulong clientId, string heroName, string rarity)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject networkObject))
        {
            Hero hero = networkObject.GetComponent<Hero>();

            HeroScriptable heroData = SoDataManager.instance.GetHeroScriptableData(heroName);

            this.ownHero = hero;
            this.holderName = heroData.heroName;
            this.heroData = heroData;
            hero.InitHeroData(heroData, this, rarity);
            ClientSetPosition();
        }
    }

}

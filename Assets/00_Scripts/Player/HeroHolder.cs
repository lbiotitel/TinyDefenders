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
    public string heroSellSFX;

    [HideInInspector] public string holderName;
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
        if (ownHero == null) return;

        heroRange.localScale = Vector2.one * ownHero.GetHeroAtkRange();
        heroRange.gameObject.SetActive(true);
    }

    public void HideHeroRange()
    {
        heroRange.gameObject.SetActive(false);
        heroRange.localScale = Vector2.zero;
    }

    public void SpawnHero(string heroName)
    {
        if (IsServer)
        {
            HeroSpawn(NetUtils.GetLocalD(), heroName);
        }
    }



    #region About Sell Hero
    public void Sell()
    {
        SoundManager.instance.PlaySfxSound(heroSellSFX,SFXTYPE.EXTRA);

        NetUtils.HostAndClientMethod(
            () => SellServerRpc(),
            () => SellHero()
         );
    }

    [ServerRpc(RequireOwnership = false)]
    private void SellServerRpc()
    {
        SellHero();
    }

    private void SellHero()
    {
        if (ownHero == null) return;
        ulong heroId = ownHero.NetworkObjectId;
        GameManager.instance.GetGold((int)(ownHero.GetHeroData().heroCost / 2f), ownHero.heroOwnerNetworkType);
        NetworkObject obj = NetworkManager.SpawnManager.SpawnedObjects[heroId];
        obj.Despawn();
        ClearHeroHolderClientRpc();
    }

    [ClientRpc]
    private void ClearHeroHolderClientRpc()
    {
        holderName = string.Empty;
        ownHero = null;
        heroData = null;

        ClientSetPosition();
        HideOwnMark();
        HideHeroRange();
        HideHeroMoveTargetMark();
    }


    #endregion


    public void SwapPosition(HeroHolder newHeroHolder)
    {
        if (ownHero == null) return;

        ownHero.SwapPosition(newHeroHolder, newHeroHolder.transform.TransformPoint(Vector2.zero));
    }

    private void HeroSpawn(ulong clientId, string heroName)
    {
        Hero hero = Instantiate(heroPrefab);

        NetworkObject networkObject = hero.GetComponent<NetworkObject>();
        networkObject.Spawn();
        hero.transform.parent = this.transform;

        UpdateHeroSpawnPositionClientRpc(networkObject.NetworkObjectId, clientId, heroName);
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
    private void UpdateHeroSpawnPositionClientRpc(ulong networkObjectId, ulong clientId, string heroName)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject networkObject))
        {
            Hero hero = networkObject.GetComponent<Hero>();

            HeroScriptable heroData = SoDataManager.instance.GetHeroScriptableData(heroName);

            this.ownHero = hero;
            this.holderName = heroData.heroName;
            this.heroData = heroData;
            hero.InitHeroData(heroData, this);
            ClientSetPosition();
        }
    }

}

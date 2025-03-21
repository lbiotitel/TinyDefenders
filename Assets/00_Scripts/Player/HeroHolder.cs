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
    [SerializeField] private Transform heroSquare;
    [SerializeField] private Transform heroCircle;

    [SerializeField] private Color[] circleColors;
    [SerializeField] private SpriteRenderer circleSpriteRenderer;

    [SerializeField] private float circleRadius;    

    [HideInInspector]public string holderName;
    [HideInInspector] public string holderId;
    [HideInInspector] public Hero ownHero = null;
    [HideInInspector] public HeroScriptable heroData;
    [HideInInspector] public int holderIndex;


    private void Start()
    {
        HideRange();
        HideCircle();
        HideSquare();
    }

    public void InitCollider()
    {
        CircleCollider2D collider = transform.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = circleRadius;
        heroSquare.GetComponent<SpriteRenderer>().size = new Vector2(circleRadius, circleRadius);
    }


    public void ShowCircle() { heroCircle.gameObject.SetActive(true); }
    public void ShowSquare() { heroSquare.gameObject.SetActive(true); }
    public void HideCircle() { heroCircle.gameObject.SetActive(false); }
    public void HideSquare() { heroSquare.gameObject.SetActive(false); }


    public void ShowRange()
    {
        float range = heroData.atkRange.GetValue();
        heroRange.localScale = Vector2.one * range * 2;

        heroRange.gameObject.SetActive(true);
    }

    public void HideRange()
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


    public void Sell(bool alarmText)
    {
        if (alarmText)
        {
            InGameUI.instance.ShowAlarmText(string.Format("영웅을 판매하였습니다. {0}{1}",
            NetUtils.GetRarityColor((HeroRarity)Enum.Parse(typeof(HeroRarity), heroData.rarity.ToString())),
            heroData.heroName));
        }

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
            HideCircle();
            HideRange();
            HideSquare();
        }
    }

    public void Composition()
    {
        List<HeroHolder> holders = new List<HeroHolder>();

        holders.Add(this);

        foreach (var holder in Spawner.instance.heroHolders)
        {
            if (holder.Value.holderName == holderName && holder.Value != this)
            {
                string compare = NetUtils.GetLocalD() == (ulong)0 ? "HOST" : "CLIENT";
                if (holder.Value.holderId.Contains(compare))
                {
                    holders.Add(holder.Value);
                }
            }
        }

        int cnt = 0;
        string[] compositionHero = new string[2];

        for (int i = 0; i < holders.Count; i++)
        {
            if (holders[i].ownHero != null)
            {
                compositionHero[cnt] = holders[i].holderId;
                cnt++;
                if (cnt >= 2)
                {
                    break;
                }
            }
        }

        for (int i = 0; i < compositionHero.Length; i++)
        {
            if (compositionHero[i] == "" || compositionHero[i] == null)
            {
                return;
            }
        }

        for (int i = 0; i < compositionHero.Length; i++)
        {
            Spawner.instance.heroHolders[compositionHero[i]].Sell(false);
        }

        HideRange();
        HideCircle();
        HideSquare();

        Spawner.instance.Summon("Uncommon");
    }


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

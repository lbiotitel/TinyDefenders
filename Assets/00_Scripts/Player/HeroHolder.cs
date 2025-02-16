using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HeroHolder : NetworkBehaviour
{
    [SerializeField] private Hero heroPrefab;
    [SerializeField] private Transform heroRange;
    [SerializeField] private Transform heroSquare;
    [SerializeField] private Transform heroCircle;
    [SerializeField] private GameObject heroCanvas;

    [SerializeField] private Button sellButton;
    [SerializeField] private Button compositionButton;

    public string holderName;
    public string holderId;
    public List<Hero> heroList = new List<Hero>();
    public HeroData heroData;
    public int holderIndex;


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
        InitButtons();
        HideRange();
        HideCircle();
        HideSquare();
    }

    private void InitButtons()
    {
        sellButton.onClick.AddListener(() => { Sell(true); });
        compositionButton.onClick.AddListener(() => { Composition(); });
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
        heroCanvas.gameObject.SetActive(true);
    }

    public void HideRange()
    {
        heroRange.gameObject.SetActive(false);
        heroCanvas.gameObject.SetActive(false);
        heroRange.localScale = Vector2.zero;
    }

    public void SpawnHero(HeroData heroData, string rarity)
    {
        if (heroList.Count == 0)
        {
            holderName = heroData.heroName;
            this.heroData = heroData;
        }
        if (IsServer)
        {
            HeroSpawn(NetUtils.GetLocalD(), heroData, rarity);
        }
    }


    private void Sell(bool alarmText)
    {
        if (alarmText)
        {
            MainUI.instance.ShowAlarmText(string.Format("영웅을 판매하였습니다. {0}{1}",
            NetUtils.GetRarityColor((Rarity)Enum.Parse(typeof(Rarity), heroData.rarity)),
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
        Hero hero = heroList[heroList.Count - 1];
        ulong heroId = hero.NetworkObjectId;
        NetworkObject obj = NetworkManager.SpawnManager.SpawnedObjects[heroId];
        SellClientRpc(heroId, clientId);
        obj.Despawn();
    }

    [ClientRpc]
    private void SellClientRpc(ulong heroKey, ulong clientId)
    {
        NetworkObject obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[heroKey];
        heroList.Remove(obj.GetComponent<Hero>());
        if (heroList.Count == 0)
        {
            DespawnHolderServerRpc(clientId);
        }
        ClientSetPosition();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnHolderServerRpc(ulong clientId)
    {
        DespawnHolderClientRpc(clientId);
        NetworkObject holder = NetworkManager.Singleton.SpawnManager.SpawnedObjects[NetworkObjectId];
        holder.Despawn();
    }

    [ClientRpc]
    private void DespawnHolderClientRpc(ulong clientId)
    {
        Spawner.instance.heroHolders.Remove(this.holderId);
        if (NetUtils.CheckId(clientId))
        {
            Spawner.instance.playerSpawnList[holderIndex].isSpawned = false;
        }
        else
        {
            Spawner.instance.friendSpawnList[holderIndex].isSpawned = false;
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
        bool full = false;
        for (int i = 0; i < holders.Count; i++)
        {
            for (int j = 0; j < holders[i].heroList.Count; j++)
            {
                if (holders[i].heroList.Count > 0)
                {
                    compositionHero[cnt] = holders[i].holderId;
                    cnt++;
                    if (cnt >= 2)
                    {
                        full = true;
                        break;
                    }
                }
            }
            if (full) break;
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

        Spawner.instance.Summon("Uncommon", true);
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
            heroList[i].SwapPosition(newHeroHolder, pos, i);
        }
    }

    private void HeroSpawn(ulong clientId, HeroData data, string rarity)
    {
        Hero hero = Instantiate(heroPrefab);
        NetworkObject networkObject = hero.GetComponent<NetworkObject>();
        networkObject.Spawn();

        hero.transform.parent = this.transform;

        UpdateHeroSpawnPositionClientRpc(networkObject.NetworkObjectId, clientId, data, rarity);
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
    private void UpdateHeroSpawnPositionClientRpc(ulong networkObjectId, ulong clientId, HeroData heroData, string rarity)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject networkObject))
        {
            Hero hero = networkObject.GetComponent<Hero>();

            heroList.Add(hero);
            networkObject.GetComponent<Hero>().InitHeroData(heroData, this, rarity);
            ClientSetPosition();
        }
    }

}

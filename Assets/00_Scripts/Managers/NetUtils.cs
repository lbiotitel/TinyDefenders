using System;
using Unity.Netcode;
using UnityEngine;

public class NetUtils
{
    public static bool IsServer()
    {
        return NetworkManager.Singleton.IsServer;
    }
    public static ulong GetLocalD()
    {
        return NetworkManager.Singleton.LocalClientId;
    }

    public static void HostAndClientMethod(Action clientAction, Action serverAction)
    {
        if (NetworkManager.Singleton.IsClient) clientAction?.Invoke();
        else if (NetworkManager.Singleton.IsServer) serverAction?.Invoke();
    }

    public static bool TryGetSpawnedObject(ulong networkObjectId, out NetworkObject spawnedObject)
    {
        return NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out spawnedObject);
    }

    public static bool CheckId(ulong clientId)
    {
        if (GetLocalD() == clientId) return true;
        return false;
    }

    public static bool IsMineHero(Hero hero, Hero targetHero)
    {
        string heroHolderId = hero.GetHeroHolder().holderId.Contains("CLIENT") ? "CLIENT" : "HOST";
        string targetHeroId = targetHero.GetHeroHolder().holderId.Contains("CLIENT") ? "CLIENT" : "HOST";

        if (heroHolderId == targetHeroId) return true;

        return false;
    }

    public static string GetRarityColor(HeroRarity rarity)
    {
        switch (rarity)
        {
            case HeroRarity.Normal:
                return "<color=#A4A4A4>";
            case HeroRarity.Uncommon:
                return "<color=#79FF73>";
            case HeroRarity.Rare:
                return "<color=#6EE5FF>";
            case HeroRarity.Legend:
                return "<color=#FFBA13>";
            case HeroRarity.God:
                return "<color=#92DC00>";
        }

        return "";
    }
}

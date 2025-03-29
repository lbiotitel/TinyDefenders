using System;
using Unity.Netcode;

public class NetUtils
{
    public static bool IsHost()
    {
        return NetworkManager.Singleton.IsServer;
    }
    public static ulong GetLocalD()
    {
        return NetworkManager.Singleton.LocalClientId;
    }

    public static PlayerNetworkType GetPlayerNetworkType()
    {
        return IsHost() ? PlayerNetworkType.HOST : PlayerNetworkType.CLIENT;
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

    public static bool IsServerHolder(Hero hero)
    {
        return hero.GetHeroHolder().holderId.Contains("HOST") ? true : false;
    }

}

using System;
using Unity.Netcode;
using UnityEngine;

public class NetUtils
{
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

    public static string GetRarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Normal:
                return "<color=#A4A4A4>";
            case Rarity.Uncommon:
                return "<color=#79FF73>";
            case Rarity.Unique:
                return "<color=#6EE5FF>";
            case Rarity.Legend:
                return "<color=#FFBA13>";
        }

        return "";
    }
}

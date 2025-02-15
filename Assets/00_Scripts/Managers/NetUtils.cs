using System;
using Unity.Netcode;

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
}

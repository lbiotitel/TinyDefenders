using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPoolingManager : NetworkBehaviour
{
    public static NetworkPoolingManager instance = null;
    [SerializeField] private GameObject networkObjectPrefab;
    [SerializeField] private List<NetworkObject> poolList = new List<NetworkObject>();
    private NetworkPrefabHandler networkPrefabHandler;

    private void Awake()
    {
        if (NetUtils.IsHost())
        {
            if (instance == null)
            {
                instance = this;
            }

            if (NetworkManager.Singleton != null)
            {
                networkPrefabHandler = new NetworkPrefabHandler(this);
                NetworkManager.Singleton.PrefabHandler.AddHandler(networkObjectPrefab, networkPrefabHandler);
            }
        }
    }

    public NetworkObject GetObject(Vector3 position, Quaternion rotation)
    {
        NetworkObject networkObject = null;

        if (poolList.Count > 0)
        {
            networkObject = poolList[0];
            poolList.RemoveAt(0);
        }
        else {
            networkObject = Instantiate(networkObjectPrefab, position, rotation).GetComponent<NetworkObject>();
        }

        GameObject go = networkObject.gameObject;
        go.gameObject.SetActive(true);
        go.transform.SetParent(null);
        networkObject.Spawn();

        go.transform.position = position;
        go.transform.rotation = rotation;

        return networkObject;
    }


    public void ReturnObject(NetworkObject networkObject)
    {
        GameObject go = networkObject.gameObject;
        go.transform.position = transform.position;
        go.gameObject.SetActive(false);
        poolList.Add(networkObject);
    }

    public void ReleaseAll()
    {
        NetworkObject[] networkObjectList = new NetworkObject[poolList.Count];

        int index = 0;
        foreach (var data in poolList) networkObjectList[index++] = data.GetComponent<NetworkObject>();

        for (int i = 0; i < networkObjectList.Length; i++)
        {
            if (networkObjectList[i] != null && networkObjectList[i].IsSpawned)
                networkObjectList[i].Despawn(true);
        }

        foreach (var obj in FindObjectsByType<Monster>(FindObjectsSortMode.None))
        {
            NetworkObject networkObject = obj.GetComponent<NetworkObject>();
            if (networkObject != null && networkObject.IsSpawned)
            {
                networkObject.Despawn(true);
            }
        }
    }

    private class NetworkPrefabHandler : INetworkPrefabInstanceHandler
    {
        private NetworkPoolingManager pool;

        public NetworkPrefabHandler(NetworkPoolingManager pool)
        {
            this.pool = pool;
        }

        public void Destroy(NetworkObject networkObject)
        {
            pool.ReturnObject(networkObject);
        }

        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            return pool.GetObject(position,rotation);
        }
    }


    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton != null) NetworkManager.Singleton.PrefabHandler.RemoveHandler(networkObjectPrefab);
        instance = null;
    }
}

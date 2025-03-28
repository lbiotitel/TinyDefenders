using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPoolingManager : NetworkBehaviour
{
    public static NetworkPoolingManager instance = null;
    [SerializeField] private GameObject networkObjectPrefab;
    [SerializeField] private List<NetworkObject> poolList = new List<NetworkObject>();

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
                NetworkManager.Singleton.PrefabHandler.AddHandler(
                    networkObjectPrefab, new NetworkPrefabHandler(this));
            }
        }
        else {
            Destroy(gameObject);
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
}

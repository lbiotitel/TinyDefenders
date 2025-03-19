using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPoolingManager : NetworkBehaviour
{
    public static NetworkPoolingManager instance = null;
    [SerializeField] private GameObject networkObjectPrefab;
    private Queue<NetworkObject> poolQueue = new Queue<NetworkObject>();

    private void Awake()
    {
        if (NetUtils.IsServer())
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

        if (poolQueue.Count > 0)
        {
            networkObject = poolQueue.Dequeue();
        }
        else {
            networkObject = Instantiate(networkObjectPrefab, position, rotation).GetComponent<NetworkObject>();
        }

        GameObject go = networkObject.gameObject;
        go.gameObject.SetActive(true);
        networkObject.Spawn();

        go.transform.position = position;
        go.transform.rotation = rotation;

        return networkObject;
    }


    public void ReturnObject(NetworkObject networkObject)
    {
        GameObject go = networkObject.gameObject;
        go.gameObject.SetActive(false);
        poolQueue.Enqueue(networkObject);
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

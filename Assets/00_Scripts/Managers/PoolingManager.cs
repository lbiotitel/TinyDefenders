using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Utilities;

public enum PoolType { 
    GAMEOBJECT,
    NETWORKOBJECT
}

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager instance;

    [Serializable]
    public class PoolObject {
        public string poolTag;
        public GameObject poolPrefab;
        public int size;
        public PoolType poolType;
    }

    [SerializeField] private PoolObject[] poolObjects;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (var poolData in poolObjects)
        { 
            int size = poolData.size;
            
            Queue<GameObject> queue = new Queue<GameObject>();
            poolDictionary[poolData.poolTag] = queue;

            for (int i = 0; i < size; i++)
            {
                queue.Enqueue(SpawnObject(poolData.poolTag, transform.position));
            }
        }
    }

    public GameObject GetObject(string tag, Vector3 position)
    {
        if (poolDictionary[tag].Count > 0)
        {
            GameObject go = poolDictionary[tag].Dequeue();
            TurnOn(ref go);
            return go;
        }

        GameObject g = SpawnObject(tag, position);
        return TurnOn(ref g);
    }

    public T GetObject<T>(string tag,Vector3 position) where T : Component
    {
     
        if (poolDictionary[tag].Count > 0)
        {
            GameObject go = poolDictionary[tag].Dequeue();
            TurnOn(ref go);
            go.transform.position = position;
            T goComponent = null;
            if (go.TryGetComponent<T>(out goComponent))
            {
                return goComponent;
            }
            return null;
        }

        T component = SpawnObject<T>(tag, position);
        
        return component;
    }


    private T SpawnObject<T>(string tag, Vector3 position) where T : Component
    { 
        return SpawnObject<T>(tag, position,Quaternion.identity);
    }

    private GameObject SpawnObject(string tag, Vector3 position)
    {
        GameObject go = SpawnObject(tag, position, Quaternion.identity);
        return go;
    }

    private GameObject SpawnObject(string tag, Vector3 position, Quaternion quaternion)
    {
        GameObject prefab = GetPrefabByTag(tag);

        if (prefab == null)
        {
            Debug.LogError("존재하지 않는 태그");
            return null;
        }
        GameObject go = Instantiate(prefab,position,quaternion);

        go.transform.position = transform.position;
        go.transform.SetParent(transform);
        go.SetActive(false);

        poolDictionary[tag].Enqueue(go);

        return go;
    }

    private T SpawnObject<T>(string tag, Vector3 position, Quaternion quaternion)where T : Component {

        GameObject go = SpawnObject(tag,position,quaternion);

        T component = null;
        if (go.TryGetComponent<T>(out component))
        {
            return component;
        }
        else {
            Debug.LogError("오브젝트의 컴포넌트가 존재하지 않음");
            return null;
        }
    }


    private GameObject GetPrefabByTag(string tag) 
    {
        foreach (var data in poolObjects)
        {
            if (data.poolTag == tag)
            {
                return data.poolPrefab;
            }
        }

        return null;
    }

    private GameObject TurnOn(ref GameObject gameObject)
    {
        gameObject.SetActive(true);
        gameObject.transform.SetParent(null);
        return gameObject;
    }

    public void ReturnObject(GameObject gameObject)
    {
        string tag = gameObject.tag;

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError("존재하지 않는 태그");
            return;
        }

        gameObject.transform.SetParent(transform);
        gameObject.transform.position = transform.position;
        if (gameObject.activeSelf) gameObject.SetActive(false);

        poolDictionary[tag].Enqueue(gameObject);
    }

}

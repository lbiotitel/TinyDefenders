using System;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
    GAMEOBJECT,
    NETWORKOBJECT
}

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager instance;

    [Serializable]
    public class PoolObject
    {
        public string poolTag;
        public GameObject poolPrefab;
        public int size;
        public PoolType poolType;
    }

    [SerializeField] private float objectClearInterval;
    [SerializeField] private int objectClearStandardTime;
    private float currentTime = 0f;
    [SerializeField] private PoolObject[] poolObjects;
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;
    private Dictionary<GameObject, GameObject> instanceToPrefabMap = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, DateTime> clearObjectDic = new Dictionary<GameObject, DateTime>();


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

    private void Update()
    {
        CheckObjectClearTime();
    }

    private void CheckObjectClearTime()
    {
        if (currentTime < objectClearInterval)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            currentTime = 0f;
            ClearObject();
        }
    }

    private void ClearObject()
    {
        DateTime dateTime = DateTime.Now;
        List<GameObject> removeKey = new List<GameObject>();

        foreach (var data in clearObjectDic)
        {
            var timeDiff = dateTime - data.Value;
            if (timeDiff.Minutes > objectClearStandardTime)
            {
                Queue<GameObject> queue = poolDictionary[data.Key];
                while (queue.Count > 0)
                {
                    GameObject go = queue.Dequeue();
#if UnityEidtor
                    Debug.Log(go.name + "오브젝트 파괴");
#endif
                    instanceToPrefabMap.Remove(go);
                    Destroy(go);
                }
            }
        }
    }

    private void Init()
    {
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
        foreach (var poolData in poolObjects)
        {
            int size = poolData.size;

            Queue<GameObject> queue = new Queue<GameObject>();
            poolDictionary[poolData.poolPrefab] = queue;

            for (int i = 0; i < size; i++)
            {
                SpawnObject(poolData.poolPrefab, transform.position);
            }
        }
    }

    public GameObject GetObject(GameObject gameObject, Vector3 position)
    {
        if (poolDictionary.ContainsKey(gameObject))
        {
            if (poolDictionary[gameObject].Count > 0)
            {
                GameObject go = poolDictionary[gameObject].Dequeue();
                TurnOn(ref go);
                go.transform.position = position;
                return go;
            }
        }
        else
        {
            poolDictionary.Add(gameObject, new Queue<GameObject>());
            clearObjectDic.Add(gameObject, DateTime.Now);
        }

        SpawnObject(gameObject, position);
        GameObject g = poolDictionary[gameObject].Dequeue();
        TurnOn(ref g);
        g.transform.position = position;

        clearObjectDic[gameObject] = DateTime.Now;

        return g;
    }

    public T GetObject<T>(GameObject gameObject, Vector3 position) where T : Component
    {
        GameObject go = GetObject(gameObject, position);
        TurnOn(ref go);
        go.transform.position = position;
        T goComponent = null;
        if (go.TryGetComponent<T>(out goComponent))
        {
            return goComponent;
        }
        return null;
    }


    private T SpawnObject<T>(GameObject gameObject, Vector3 position) where T : Component
    {
        return SpawnObject<T>(gameObject, position, Quaternion.identity);
    }

    private GameObject SpawnObject(GameObject gameObject, Vector3 position)
    {
        GameObject go = SpawnObject(gameObject, position, Quaternion.identity);
        return go;
    }

    private GameObject SpawnObject(GameObject gameObject, Vector3 position, Quaternion quaternion)
    {
        GameObject go = Instantiate(gameObject, position, quaternion);

        go.transform.position = transform.position;
        go.transform.SetParent(transform);
        go.SetActive(false);

        poolDictionary[gameObject].Enqueue(go);
        instanceToPrefabMap[go] = gameObject;

        return go;
    }

    private T SpawnObject<T>(GameObject gameObject, Vector3 position, Quaternion quaternion) where T : Component
    {

        GameObject go = SpawnObject(gameObject, position, quaternion);

        T component = null;
        if (go.TryGetComponent<T>(out component))
        {
            return component;
        }
        else
        {
#if UnityEidtor
            Debug.LogError("오브젝트의 컴포넌트가 존재하지 않음");
#endif
            return null;
        }
    }



    private GameObject TurnOn(ref GameObject gameObject)
    {
        gameObject.SetActive(true);
        gameObject.transform.SetParent(null);
        return gameObject;
    }

    public void ReturnObject(GameObject gameObject)
    {
        if (instanceToPrefabMap.TryGetValue(gameObject, out GameObject prefab))
        {
            gameObject.transform.SetParent(transform);
            gameObject.transform.position = transform.position;
            if (gameObject.activeSelf) gameObject.SetActive(false);

            poolDictionary[prefab].Enqueue(gameObject);
        }
        else
        {
#if UnityEidtor
            Debug.LogError("풀링 되지 않은 객체" + gameObject.name);
#endif
        }
    }

}

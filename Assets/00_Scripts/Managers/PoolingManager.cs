using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType { 
    MONSTER,
    DAMAGETEXT
}

public class PoolingManager : MonoBehaviour
{
    private static PoolingManager instance;

    [Serializable]
    public class PoolObject {
        public string poolTag;
        public GameObject poolPrefab;
        public int size;
    }

    [SerializeField] private PoolObject[] poolObjects;
    private List<GameObject> spawnedObjects;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private void Awake()
    {
        if (NetUtils.IsServer())
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
        else {
            Destroy(gameObject);
        }
    }


   

}

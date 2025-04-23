using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RelicContainer : MonoBehaviour
{
    private List<RelicSO> relicList = new List<RelicSO>();


    private void Start()
    {
        EventManager.instance.onRelicSelectAction += AddRelic;
    }

    private void AddRelic(RelicSO relicSO)
    {
        relicList.Add(relicSO);
    }

    public List<RelicSO> GetRelicList()
    { 
        return relicList;
    }
}

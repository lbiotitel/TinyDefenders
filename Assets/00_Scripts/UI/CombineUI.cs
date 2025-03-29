using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombineUI : MonoBehaviour
{
    private CombineSciptable[] combineDatas;
    private List<CombineNodeUI> combineNodeUIs = new List<CombineNodeUI>();
    [SerializeField] private Transform contentParent;

    [SerializeField] private CombineNodeUI combineNodeUI;

    private void Awake()
    {
        InitCombineUI();
    }

    private void OnEnable()
    {
        foreach (var nodeUI in combineNodeUIs)
        {
            nodeUI.CanCombineHero();
        }
    }

    private void InitCombineUI()
    {
        combineDatas = Resources.LoadAll<CombineSciptable>("CombineHeroData");

        SetAllCombineUI();
    }

    private void SetAllCombineUI()
    {
        foreach (var combineData in combineDatas)
        {
            CombineNodeUI nodeUI = Instantiate(combineNodeUI);
            nodeUI.transform.SetParent(contentParent,false);
            nodeUI.SetHeroData(combineData.mainHeroData,combineData.subHeroDatas);

            combineNodeUIs.Add(nodeUI);
        }
    }
}

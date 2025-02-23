using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombineUI : MonoBehaviour
{
    private CombineSciptable[] combineDatas;
    private List<CombineNodeUI> combineNodeUIs = new List<CombineNodeUI>();
    [SerializeField] private Transform contentParent;

    [SerializeField] private CombineNodeUI combineNodeUI;
    [SerializeField] private GameObject heroImageObject;
    [SerializeField] private GameObject plusObject;
    [SerializeField] private GameObject equalObject;

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
            nodeUI.transform.SetParent(contentParent);

            for (int i = 0; i < combineData.subHeroDatas.Count; i++)
            {
                GameObject go = Instantiate(heroImageObject);
                go.GetComponent<Image>().sprite = combineData.subHeroDatas[i].heroMainImage;
                nodeUI.AddHeroUI(go);

                if (i != combineData.subHeroDatas.Count - 1)
                {
                    GameObject po = Instantiate(plusObject);
                    nodeUI.AddHeroUI(po);
                }
            }

            GameObject eo = Instantiate(equalObject);
            nodeUI.AddHeroUI(eo);

            GameObject mo = Instantiate(heroImageObject);
            mo.GetComponent<Image>().sprite = combineData.mainHeroData.heroMainImage;
            nodeUI.AddHeroUI(mo);
            nodeUI.SetName(combineData.mainHeroData.heroName);
            nodeUI.SetHeroData(combineData.mainHeroData,combineData.subHeroDatas);

            combineNodeUIs.Add(nodeUI);
        }
    }
}

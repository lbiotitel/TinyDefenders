using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class FilterNode {
    public Sprite filterImage;
    public HeroTribe filterTribe;
}

public class CombineUI : MonoBehaviour
{
    private List<CombineNodeUI> combineNodeUIs = new List<CombineNodeUI>();
    [SerializeField] private Transform contentParent;
    [SerializeField] private CombineNodeUI combineNodeUIPrefab;
    private Animator animator;

    [Header("Combine Hero Info UI")]
    [SerializeField] private CombineHeroInfoUI combineHeroInfoUI;

    [Header("Combine Filter")]
    [SerializeField] private Button filterButton;
    [SerializeField] private Image filterImage;
    [SerializeField] private FilterNode[] filterNodes;
    private int filterIndex = 0;


    private void Awake()
    {
        InitCombineUI();
        animator = GetComponent<Animator>();
        filterButton.onClick.AddListener(ClickFilterNode);
        filterImage.sprite = filterNodes[filterIndex].filterImage;
    }

    private void OnEnable()
    {
        foreach (var nodeUI in combineNodeUIs)
        {
            nodeUI.CanCombineHero(null);
        }
    }

    private void InitCombineUI()
    {
        SetAllCombineUI();
    }

    private void ClickFilterNode()
    {
        filterIndex = (filterIndex + 1) % filterNodes.Length;
        filterImage.sprite = filterNodes[filterIndex].filterImage;
        FilterCombineUINode(filterNodes[filterIndex].filterTribe);
    }

    private void FilterCombineUINode(HeroTribe filterTribe)
    {
        foreach (var combineUI in combineNodeUIs)
        {
            combineUI.FilterNode(filterTribe);
        }
    }

    private void SetAllCombineUI()
    {
        List<CombineSciptable> combineDatas = GameResourceManager.instance.GetAllCombineData();

        foreach (var combineData in combineDatas)
        {
            CombineNodeUI nodeUI = Instantiate(combineNodeUIPrefab);
            nodeUI.transform.SetParent(contentParent, false);
            nodeUI.SetHeroData(combineData.mainHeroData, combineData.subHeroDatas, this);

            combineNodeUIs.Add(nodeUI);
        }
    }

    public void ShowHeroCombineUI()
    {
        gameObject.SetActive(true);
    }

    public void HideHeroCombineUI()
    {
        animator.SetTrigger("HIDE");
    }

    public void ShowHeroInfoUI(HeroScriptable hero,List<HeroScriptable> subHeroList, Transform transform)
    {
        combineHeroInfoUI.ShowCombineHeroInfoUI(hero,subHeroList, transform);
    }

    public void HideHeroInfoUI()
    {
        combineHeroInfoUI.HideCombineHeroInfoUI();
    }
}

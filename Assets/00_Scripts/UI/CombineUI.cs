using System.Collections.Generic;
using UnityEngine;

public class CombineUI : MonoBehaviour
{
    private List<CombineNodeUI> combineNodeUIs = new List<CombineNodeUI>();
    [SerializeField] private Transform contentParent;
    [SerializeField] private CombineNodeUI combineNodeUI;
    private Animator animator;

    [Header("Combine Hero Info UI")]
    [SerializeField] private CombineHeroInfoUI combineHeroInfoUI;


    private void Awake()
    {
        InitCombineUI();
        animator = GetComponent<Animator>();
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
        SetAllCombineUI();
    }

    private void SetAllCombineUI()
    {
        List<CombineSciptable> combineDatas = SoDataManager.instance.GetAllCombineData();

        foreach (var combineData in combineDatas)
        {
            CombineNodeUI nodeUI = Instantiate(combineNodeUI);
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

    public void ShowHeroInfoUI(HeroScriptable hero, Transform transform)
    {
        combineHeroInfoUI.ShowCombineHeroInfoUI(hero, transform);
    }

    public void HideHeroInfoUI()
    {
        combineHeroInfoUI.HideCombineHeroInfoUI();
    }
}

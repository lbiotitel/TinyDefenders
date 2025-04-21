using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class HeroSynergyUI : MonoBehaviour
{
    [Header("Synergy Info UI Node")]
    [SerializeField] private List<HeroSynergyUINode> synergyUINodes;
    [SerializeField] private HeroSynergyUINode heroSynergyUIPrefab;
    [SerializeField] private Transform contentParent;
    private RectTransform rectTransform;

    [Header("Synergy Info UI")]
    [SerializeField] private GameObject synergyInfoObject;
    [SerializeField] private TextMeshProUGUI synergyNameText;
    [SerializeField] private TextMeshProUGUI synergyDescText;

    private void Start()
    {
        InitHeroSynergyUI();
        HideSynergyInfo();
        rectTransform = synergyInfoObject.GetComponent<RectTransform>();
        EventManager.instance.onHeroSynergyChangedAction += ShowHeroSynergyUI;
    }

    private void InitHeroSynergyUI()
    {
        List<HeroSynergySOBase> heroSynergySos = GameResourceManager.instance.GetHeroSynergySOs();

        for (int i = 0; i < heroSynergySos.Count; i++)
        {
            HeroSynergyUINode heroSynergyUINode = Instantiate(heroSynergyUIPrefab);
            heroSynergyUINode.transform.SetParent(contentParent, false);
            heroSynergyUINode.InitSynergyUINode(heroSynergySos[i], this);

            synergyUINodes.Add(heroSynergyUINode);
        }
    }


    public void ShowHeroSynergyUI()
    {
        gameObject.SetActive(true);

        List<HeroSynergySOBase> heroSynergySOs = PlayerStatManager.instance.GetHeroSynergyData(NetUtils.GetPlayerNetworkType());

        foreach (var data in heroSynergySOs)
        {
            Debug.Log(data.synergyName + " : " + data.GetCurrentSynergyLevel());
            ShowHeroSynergyUI(data);
        }
    }


    private void ShowHeroSynergyUI(HeroSynergySOBase heroSynergySOBase)
    {
        for (int i = 0; i < synergyUINodes.Count; i++)
        {
            if (synergyUINodes[i].heroSynergySO.synergyName == heroSynergySOBase.synergyName)
            {
                if (heroSynergySOBase.GetHeroCount() > 0)
                {
                    synergyUINodes[i].ShowSynergyUINode(heroSynergySOBase.GetCurrentSynergyLevel());
                }
                else
                {
                    synergyUINodes[i].HideSynergyUINode();
                }
                break;
            }
        }
    }


    public void ShowSynergyInfo(Transform transform, string synergyName, string synergyDesc)
    {
        Utils.ModifyTransform(transform,ref rectTransform);
        synergyNameText.text = synergyName;
        synergyDescText.text = synergyDesc;
        LayoutRebuilder.ForceRebuildLayoutImmediate(synergyInfoObject.GetComponent<RectTransform>());
    }

    public void HideSynergyInfo()
    {
        synergyInfoObject.transform.position = new Vector2(-1000, -1000);
    }

}

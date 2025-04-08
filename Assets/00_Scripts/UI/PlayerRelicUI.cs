using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerRelicUI : MonoBehaviour
{
    [SerializeField] private RelicUINode relicUINodePrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private TMP_Text relicPointText;
    [SerializeField] private Button saveRelicButton;
    [SerializeField] private RelicInfoUI relicInfoUI;
    private int relicPoint;
    public UnityAction<int> onRelicPointChanged;
    private List<GameObject> relicUINodes = new List<GameObject>();
    private List<RelicSO> relicList = new List<RelicSO>();
    private void Start()
    {
        saveRelicButton.onClick.AddListener(SaveRelicData);
        onRelicPointChanged += RelicPointText;
    }

    private void OnEnable()
    {
        InitRelicUI();
        relicPoint = CloudManager.instance.GetUserData().relicPoint;
        relicPointText.text = relicPoint.ToString();
    }


    private void SaveRelicData()
    {
        foreach (var relic in relicList)
        {
            CloudManager.instance.SetRelicState(relic.relicName,relic.IsRelicOn());
        }

        CloudManager.instance.SaveUserData();
    }


    private void InitRelicUI()
    {
        if (relicUINodes.Count > 0)
            relicUINodes.ForEach(x => Destroy(x));

        relicList = SoDataManager.instance.GetAllRelicData();
        Dictionary<string,bool> ownRelicData = CloudManager.instance.GetUserData().relicDataDic;

        foreach (var relic in relicList)
        {
            RelicUINode relicUINode = Instantiate(relicUINodePrefab, contentParent);
            relicUINode.InitRelic(relic,this);
            relicUINodes.Add(relicUINode.gameObject);
            if (ownRelicData[relic.relicName])
                relicUINode.RelicOn();
            else 
                relicUINode.RelicOff();
        }
    }

    public void ShowRelicInfoUI(Transform targetTransform,string relicName, string relicDesc, int requirePoint, int requireLevel)
    {
        relicInfoUI.ShowRelicInfo(targetTransform, relicName,relicDesc,requirePoint, requireLevel);
    }

    private void RelicPointText(int relicPoint)
    { 
        relicPointText.text = relicPoint.ToString();
    }

    public void HideRelicInfoUI()
    {
        relicInfoUI.HideRelicInfo();
    }

}

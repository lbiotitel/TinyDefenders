using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text relicNameText;
    [SerializeField] private TMP_Text relicDescriptionText;
    [SerializeField] private TMP_Text requireLevelText;
    [SerializeField] private TMP_Text relicRequirePointText;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    public void ShowRelicInfo(Transform targetTransform,string relicName, string relicDescription, int requirePoint,int requireLevel)
    {
        gameObject.SetActive(true);

        Utils.ModifyTransform(targetTransform, ref rectTransform);

        relicNameText.text = relicName;
        relicDescriptionText.text = relicDescription;
        requireLevelText.text = "Require Level : " + requireLevel.ToString();
        relicRequirePointText.text = "Require Point: " + requirePoint.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public void HideRelicInfo()
    { 
        gameObject.SetActive(false);
    }
}

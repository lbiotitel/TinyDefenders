using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropDown;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Toggle isWindowToggleButton;

    private int resolutionIndex;

    private void Awake()
    {
        resolutionDropDown.ClearOptions();
        resolutionDropDown.onValueChanged.AddListener(SelectResolution);
        applyButton.onClick.AddListener(ApplyOption);
        cancelButton.onClick.AddListener(HideOptionUI);
        InitDropDownOptions();
    }

    private void OnEnable()
    {
        resolutionDropDown.value = GameSettingManager.instance.gameSettingData.resolutionIndex;
        isWindowToggleButton.isOn = GameSettingManager.instance.gameSettingData.isWindow;
    }

    private void InitDropDownOptions()
    {
        List<string> options = new List<string>();

        for (int i = 0; i < ResolutionData.resolutionList.Count; i++)
        {
            options.Add(string.Format("{0:0} x {1:0}",
                ResolutionData.resolutionList[i].x, ResolutionData.resolutionList[i].y));
        }

        resolutionDropDown.AddOptions(options);
    }

    private void SelectResolution(int index)
    {
        resolutionIndex = index;
    }

    private void ApplyOption()
    {
        GameSettingManager.instance.SetResolution(resolutionIndex, isWindowToggleButton.isOn);
        Screen.SetResolution(
            (int)ResolutionData.resolutionList[resolutionIndex].x,
            (int)ResolutionData.resolutionList[resolutionIndex].y,
            isWindowToggleButton.isOn);

        HideOptionUI();
    }

    private void HideOptionUI()
    {
        gameObject.SetActive(false);
    }
}

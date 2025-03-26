using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private Button startMatchingButton;
    [SerializeField] private Button cancleButton;
    [SerializeField] private GameObject matchingUI;
    [SerializeField] private NetManager netManager;

    private void Awake()
    {
        InitButtons();
    }

    private void InitButtons()
    {
        startMatchingButton.onClick.AddListener(() =>
        {
            netManager.StartMatching();
            ShowMathcingUI();
        });

        cancleButton.onClick.AddListener(() => {
            netManager.DestroyLobby();
            HideMatchingUI();
        });
    }

    public void ShowMathcingUI()
    { 
        matchingUI.SetActive(true);
    }

    private void HideMatchingUI()
    {
        matchingUI.SetActive(false);
    }
}

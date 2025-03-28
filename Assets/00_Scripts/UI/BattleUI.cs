using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private NetManager netManager;

    [Header("Random Matching")]
    [SerializeField] private Button startMatchingButton;
    [SerializeField] private Button cancleButton;
    [SerializeField] private GameObject matchingUI;

    [Header("Friend Matching")]
    [SerializeField] private Button firendMatchingButton;
    [SerializeField] private Button createFriendMatchingButton;
    [SerializeField] private GameObject waitFriendUI;

    [SerializeField] private Button joinFriendMatchingButton;
    [SerializeField] private Button friendMatchingCancleButton;

    [SerializeField] private GameObject friendMatchingUI;
    [SerializeField] private TMP_InputField displayJoinCode;
    [SerializeField] private TMP_InputField inputJoinCode;


    private void Awake()
    {
        InitButtons();
        waitFriendUI.SetActive(false);
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

        firendMatchingButton.onClick.AddListener(ShowFriendMatchingUI);

        createFriendMatchingButton.onClick.AddListener(CreateFriendMatching);

        friendMatchingCancleButton.onClick.AddListener(DestroyFriendMatching);

        joinFriendMatchingButton.onClick.AddListener(JoinFriendMatching);
    }

    public void ShowFriendMatchingUI()
    {
        friendMatchingUI.SetActive(true);
    }

    private async void CreateFriendMatching()
    {
        waitFriendUI.gameObject.SetActive(true);
        friendMatchingCancleButton.interactable = false;
        displayJoinCode.text =string.Empty;

        string joinCode = await netManager.CreateFriendMatching();

        if (joinCode != null)
        {
            displayJoinCode.text = joinCode;
            friendMatchingCancleButton.interactable = true;
        }
        else {
            Debug.Log("초대 코드 만들기 실패");
            waitFriendUI.gameObject.SetActive(false);
        }
    }

    private void DestroyFriendMatching()
    {
        if (netManager.DestroyFriendMatching())
        {
            waitFriendUI.gameObject.SetActive(false);
        }
    }

    private async void JoinFriendMatching()
    {
        string joinCode = inputJoinCode.text;

        if (!string.IsNullOrEmpty(joinCode))
        {
            displayJoinCode.readOnly = true;
            joinFriendMatchingButton.interactable = false;  

            bool canJoin = await netManager.JoinGameWithCode(joinCode);

            if (canJoin)
            {

            }
            else {
                displayJoinCode.readOnly = false;
                joinFriendMatchingButton.interactable = true;
            }
        }
        else { 
            //제대로된 코드를 입력하세요
        }
    }

    private void ShowMathcingUI()
    { 
        matchingUI.SetActive(true);
    }

    private void HideMatchingUI()
    {
        matchingUI.SetActive(false);
    }
}

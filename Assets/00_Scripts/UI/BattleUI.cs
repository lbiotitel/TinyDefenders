using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("Random Matching")]
    [SerializeField] private Button startMatchingButton;
    [SerializeField] private Button randomMatchingCancleButton;
    [SerializeField] private GameObject matchingUI;
    [SerializeField] private TextMeshProUGUI matchingTimeText;
    private float matchingTime = 0f;

    [Header("Friend Matching")]
    [SerializeField] private Button firendMatchingButton;
    [SerializeField] private Button createFriendMatchingButton;
    [SerializeField] private GameObject waitFriendUI;

    [SerializeField] private Button joinFriendMatchingButton;
    [SerializeField] private Button friendMatchingCancleButton;

    [SerializeField] private GameObject friendMatchingUI;
    [SerializeField] private TMP_InputField displayJoinCode;
    [SerializeField] private TMP_InputField inputJoinCode;

    [Header("DisconnectedUI")]
    [SerializeField] private GameObject disconnectedUI;


    private void Awake()
    {
        InitButtons();
        waitFriendUI.SetActive(false);
    }

    private void Update()
    {
        MathcingTimer();
    }

    private void MathcingTimer()
    {
        matchingTime += Time.deltaTime;
        matchingTimeText.text = string.Format("{0:D2}:{1:D2}", (int)(matchingTime / 60), (int)(matchingTime % 60));
    }

    private void InitButtons()
    {
        startMatchingButton.onClick.AddListener(StartRandomMathcing);

        randomMatchingCancleButton.onClick.AddListener(DestroyRandomMatching);

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

        string joinCode = await NetManager.instnace.CreateFriendMatching();

        if (joinCode != null)
        {
            displayJoinCode.text = joinCode;
            friendMatchingCancleButton.interactable = true;
        }
        else {
            #if UnityEidtor
            Debug.Log("초대 코드 만들기 실패");
#endif
            waitFriendUI.gameObject.SetActive(false);
        }
    }

    private void DestroyFriendMatching()
    {
        if (NetManager.instnace.DestroyFriendMatching())
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
            createFriendMatchingButton.interactable = false;

            bool canJoin = await NetManager.instnace.JoinGameWithCode(joinCode);

            if (canJoin)
            {
                MainSceneUI.instance.ShowMatchingCompleteUI();
            }
            else {
                displayJoinCode.readOnly = false;
                joinFriendMatchingButton.interactable = true;
                createFriendMatchingButton.interactable = true;
            }
        }
        else { 
            //제대로된 코드를 입력하세요
        }
    }


    private async void StartRandomMathcing()
    {
        ShowMathcingUI();
        matchingTime = 0f;
        randomMatchingCancleButton.interactable = false;

        bool result = await NetManager.instnace.StartRandomMatching();

        if (result)
        {
            randomMatchingCancleButton.interactable = true;
        }
        else
        {
            StartRandomMathcing();
        }
    }




    private void DestroyRandomMatching()
    {
        Debug.Log("로비 파괴");
        HideMatchingUI();
        NetManager.instnace.DestroyLobby();
        NetworkManager.Singleton.Shutdown();
    }

    public void ShowDisconnectedUI(ulong clientId)
    {
        HideMatchingUI();
    }

    private void ShowMathcingUI()
    { 
        matchingUI.SetActive(true);
    }

    private void HideMatchingUI()
    {
        matchingUI.SetActive(false);
    }


    private void OnDestroy()
    {

    }

}

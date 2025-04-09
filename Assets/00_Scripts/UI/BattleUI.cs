using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("Random Matching")]
    [SerializeField] private Button startMatchingButton;
    [SerializeField] private Button randomMatchingCancleButton;
    [SerializeField] private GameObject matchingUI;
    [SerializeField] private TextMeshProUGUI matchingTimeText;
    [SerializeField] private string startMactchingSFX;
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

    [Header("User Profile")]
    [SerializeField] private ProfileUI profileUI;
    [SerializeField] private Button profileButton;
    [SerializeField] private Slider userExpSlider;
    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private TextMeshProUGUI userLevelText;


    private void Awake()
    {
        InitButtons();
        waitFriendUI.SetActive(false);
    }

    private void Start()
    {
        LoadUserData();
    }

    private async void LoadUserData()
    {
        await NetworkLobbyManager.instance.SingInUnityService();

        var userData = await CloudManager.instance.LoadUserData();
        userNameText.text = userData.userName;
        userLevelText.text = string.Format("LV.{0}",userData.userLevel);

        int requireExp = SoDataManager.instance.GetUserRequireExp(userData.userLevel);
        userExpSlider.maxValue = requireExp;
        userExpSlider.value = userData.userExp;
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

        profileButton.onClick.AddListener(() => profileUI.ShowProfile());
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

        string joinCode = await NetworkLobbyManager.instance.CreateFriendMatching();

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
        if (NetworkLobbyManager.instance.DestroyFriendMatching())
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

            bool canJoin = await NetworkLobbyManager.instance.JoinGameWithCode(joinCode);

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
        SoundManager.instance.PlaySfxSound(startMactchingSFX,SFXTYPE.UISOUND);

        ShowMathcingUI();
        matchingTime = 0f;
        randomMatchingCancleButton.interactable = false;

        bool result = await NetworkLobbyManager.instance.StartRandomMatching();

        if (result)
        {
            randomMatchingCancleButton.interactable = true;
            startMatchingButton.interactable = false;
            NetworkManager.Singleton.OnClientDisconnectCallback += HideMatchingUIOnDisconnect;
        }
        else
        {
            StartRandomMathcing();
        }
    }

    private void HideMatchingUIOnDisconnect(ulong clientId)
    {
        HideMatchingUI();
    }


    private async void DestroyRandomMatching()
    {
        Debug.Log("로비 파괴");
        HideMatchingUI();
        bool result = await NetworkLobbyManager.instance.DestroyLobby();

        if (result)
        {
            NetworkManager.Singleton.Shutdown();
            startMatchingButton.interactable = true;
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

    private void OnDestroy()
    {
        startMatchingButton.onClick.RemoveAllListeners();
        randomMatchingCancleButton.onClick.RemoveAllListeners();
        firendMatchingButton.onClick.RemoveAllListeners();
        createFriendMatchingButton.onClick.RemoveAllListeners();
        friendMatchingCancleButton.onClick.RemoveAllListeners();
        joinFriendMatchingButton.onClick.RemoveAllListeners();
        profileButton.onClick.RemoveAllListeners();

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= HideMatchingUIOnDisconnect;
        }
    }
}

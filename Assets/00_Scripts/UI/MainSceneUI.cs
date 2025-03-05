using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneUI : MonoBehaviour
{
    public static MainSceneUI instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    [Header("NickName UI")]
    [SerializeField] private GameObject nickNameUI;
    [SerializeField] private TextMeshProUGUI nickNameText;
    [SerializeField] private Button nickNameSetButton;

    [Header("Mene Button")]
    [SerializeField] private Button battleButton; 
    [SerializeField] private Button heroButton; 
    [SerializeField] private Button guildButton; 
    [SerializeField] private Button shopButton;

    [Header("Menu UI")]
    [SerializeField] private GameObject battleUI;
    [SerializeField] private GameObject heroCollectionUI;

    [Header("Battle UI")]
    [SerializeField] private TextMeshProUGUI bestWaveText;

    

    private void Start()
    {
        nickNameSetButton.onClick.AddListener(() => SetNickName());
        InitButtoons();
    }

    public void InitMainScene()
    {
        bestWaveText.text = CloudManager.instance.userData.bestWave.ToString();
    }

    private void InitButtoons()
    {
        battleButton.onClick.AddListener(() => { 
            HideAllMenu(); 
            battleUI.SetActive(true); 
        });

        heroButton.onClick.AddListener(() =>
        {
            HideAllMenu();
            heroCollectionUI.SetActive(true);
        });

        HideAllMenu();
        battleUI.SetActive(true);
    }

    private void HideAllMenu()
    {
        battleUI.SetActive(false);
        heroCollectionUI.SetActive(false);
    }

    public void ShowNickNameUI()
    { 
        nickNameUI.SetActive(true);
    }

    public void SetNickName()
    {
        string nickName = nickNameText.text;

        if (string.IsNullOrEmpty(nickName)) return;
        if (nickName.Length < 1 || nickName.Length >= 8) return;

        CloudManager.instance.userData.userName = nickName;

        CloudManager.instance.SaveUserData();
        nickNameUI.SetActive(false);
    }
        
}

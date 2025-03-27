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
    [SerializeField] private Button spellCardButton; 
    [SerializeField] private Button rankingButton; 
    [SerializeField] private Button shopButton;

    [Header("Menu UI")]
    [SerializeField] private GameObject battleUI;
    [SerializeField] private GameObject heroCollectionUI;
    [SerializeField] private GameObject cardCollectionUI;
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject rankingUI;

    [Header("Battle UI")]
    [SerializeField] private TextMeshProUGUI bestWaveText;

    [Header("Extra")]
    [SerializeField] private Color activeButtonColor;


    private void Start()
    {
        nickNameSetButton.onClick.AddListener(() => SetNickName());
        InitButtoons();
        SoundManager.instance.PlayMusicSound("MainBgm",true);
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
            battleButton.GetComponent<Image>().color = activeButtonColor;
        });

        heroButton.onClick.AddListener(() =>
        {
            HideAllMenu();
            heroCollectionUI.SetActive(true);
            heroButton.GetComponent<Image>().color = activeButtonColor;
        });

        shopButton.onClick.AddListener(() => {
            HideAllMenu();
            shopUI.SetActive(true);
            shopButton.GetComponent<Image>().color = activeButtonColor;
        });

        rankingButton.onClick.AddListener(() =>
        {
            HideAllMenu();
            rankingUI.SetActive(true);
            rankingButton.GetComponent<Image>().color = activeButtonColor;
        });

        spellCardButton.onClick.AddListener(() =>
        {
            HideAllMenu();
            cardCollectionUI.SetActive(true);
            spellCardButton.GetComponent<Image>().color = activeButtonColor;
        });

        HideAllMenu();
        battleButton.onClick?.Invoke();
    }

    private void HideAllMenu()
    {
        battleUI.SetActive(false);
        heroCollectionUI.SetActive(false);
        shopUI.SetActive(false);
        rankingUI.SetActive(false);
        cardCollectionUI.SetActive(false);

        battleButton.GetComponent<Image>().color = Color.white;
        heroButton.GetComponent<Image>().color = Color.white;
        shopButton.GetComponent<Image>().color = Color.white;
        rankingButton.GetComponent<Image>().color = Color.white;
        spellCardButton.GetComponent<Image>().color = Color.white;
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

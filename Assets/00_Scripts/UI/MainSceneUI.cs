 using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

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
    [SerializeField] private Button relicButton;

    [Header("Menu UI")]
    [SerializeField] private GameObject battleUI;
    [SerializeField] private GameObject heroCollectionUI;
    [SerializeField] private GameObject cardCollectionUI;
    [SerializeField] private GameObject relicUI;
    [SerializeField] private GameObject rankingUI;

    [Header("Battle UI")]
    [SerializeField] private GameObject matchingCompleteUI;

    [Header("Extra")]
    [SerializeField] private Color activeButtonColor;

    [Header("Menu Button SFX")]
    [SerializeField] private string mainSceneBGM;
    [SerializeField] private string menuButtonSFX;

    private void Start()
    {
        nickNameSetButton.onClick.AddListener(() => SetNickName());
        InitButtoons();
        HideMatchingCompleteUI();
        SoundManager.instance.PlayMusicSound(mainSceneBGM, true);
    }

    private void InitButtoons()
    {
        InitButton(battleButton,battleUI);
        InitButton(heroButton, heroCollectionUI);
        InitButton(relicButton,relicUI);
        InitButton(rankingButton,rankingUI);
        InitButton(spellCardButton,cardCollectionUI);

        HideAllMenu();
        battleButton.onClick?.Invoke();
    }

    private void InitButton(Button button,GameObject buttonUI)
    {
        button.onClick.AddListener(() => {
            if (buttonUI.activeSelf) return;
            SoundManager.instance.PlaySfxSound(menuButtonSFX,SFXTYPE.UISOUND);
            HideAllMenu();
            buttonUI.SetActive(true);
            button.GetComponent<Image>().color = activeButtonColor;
        });
    }

    private void HideAllMenu()
    {
        battleUI.SetActive(false);
        heroCollectionUI.SetActive(false);
        relicUI.SetActive(false);
        rankingUI.SetActive(false);
        cardCollectionUI.SetActive(false);

        battleButton.GetComponent<Image>().color = Color.white;
        heroButton.GetComponent<Image>().color = Color.white;
        relicButton.GetComponent<Image>().color = Color.white;
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

        CloudManager.instance.GetUserData().userName = nickName;

        CloudManager.instance.SaveUserData();
        nickNameUI.SetActive(false);
    }


    public void ShowMatchingCompleteUI()
    { 
        matchingCompleteUI.SetActive(true);
    }

    public void HideMatchingCompleteUI()
    {
        matchingCompleteUI.SetActive(false);
    }
}

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

    private void Start()
    {
        nickNameSetButton.onClick.AddListener(() => SetNickName());
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

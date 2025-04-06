using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class NetworkSceneManager : MonoBehaviour
{
    public static NetworkSceneManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private GameObject messageBox;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button exitButton;

    private void Start()
    {
        exitButton.onClick.AddListener(ClickExitButton);
    }

    public void ShowNetworkMessage(NetworkMessageType networkMessageType)
    {

        if (SceneManager.GetActiveScene().name == SceneName.MainScene)
        {
            MainSceneUI.instance.HideMatchingCompleteUI();
            return;
        }

        Time.timeScale = 0f;
        messageBox.SetActive(true);
        switch (networkMessageType)
        {
            case NetworkMessageType.HOST_SHUT_DOWN:
                messageText.text = "È£½ºÆ®¿Í ¿¬°áÀÌ ²÷±è";
                break;
            case NetworkMessageType.CLIENT_DISCONNECT:
                messageText.text = "Å¬¶óÀÌ¾ðÆ®¿Í ¿¬°áÀÌ ²÷±è";
                break;
        }
    }


    private void ClickExitButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneName.MainScene);
        HideNetworkMessage();
    }

    private void HideNetworkMessage()
    {
        gameObject.SetActive(false);
    }


    private void OnApplicationQuit()
    {
        if (instance != null)
        {
            exitButton.onClick.RemoveAllListeners();
            instance = null;
        }
    }
}

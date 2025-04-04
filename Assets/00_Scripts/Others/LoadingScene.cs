using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : NetworkBehaviour
{
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private float loadingTime;
    [SerializeField] private TextMeshProUGUI loadingText;
    private NetworkVariable<float> sliderValue = new NetworkVariable<float>(0f);
    private int textIndex = -1;
    private string hostName;
    private string clientName;

    private void Awake()
    {
        SoDataManager.instance.InitDrawHeroCardProbability();
        SoDataManager.instance.InitAllHeroSynergyData();
        SoDataManager.instance.InitAllCombineData();
        SoDataManager.instance.InitInGameQuestData();
        loadingSlider.value = 0f;
    }

    private IEnumerator LoadingCoroutine()
    {
        float elapsedTime = 0f;
        float percent = 0f;
        while (percent < 1f)
        {
            elapsedTime += Time.deltaTime;
            percent = elapsedTime / loadingTime;
            LoadingTextLogic(percent);
            sliderValue.Value = percent;

            yield return null;
        }

        NetworkManager.Singleton.SceneManager.LoadScene(
            SceneName.GameScene,
            UnityEngine.SceneManagement.LoadSceneMode.Single
        );
    }

    private void LoadingTextLogic(float percent)
    {
        if (percent >= 0.1)
        {
            if (textIndex == -1)
            {
                LoadingTextClientRpc("리소스 로드 중...");
                textIndex++;
            }
        }
        if (percent >= 0.5)
        {
            if (textIndex == 0)
            {
                LoadingTextClientRpc("로딩이 거의 끝나는 중...");
                textIndex++;
            }
        }
        if (percent >= 0.9)
        {
            if (textIndex == 1)
            {
                LoadingTextClientRpc("게임시작하는 중...");
                textIndex++;
            }
        }
    }

    [ClientRpc]
    private void LoadingTextClientRpc(string text)
    {
        loadingText.text = text;
    }


    #region Send Name
    //send users name
    [ServerRpc(RequireOwnership = false)]
    private void SendUserNameServerRpc(string userName)
    {
        if (NetUtils.IsHost()) SendUserNameClientRpc(CloudManager.instance.GetUserData().userName, userName);
    }

    [ClientRpc]
    private void SendUserNameClientRpc(string hostName, string clientName)
    {
        this.hostName = hostName;
        this.clientName = clientName;
        if (NetUtils.IsHost()) StartCoroutine(LoadingCoroutine());
    }


    public void StartLoadingScene()
    {
        if (!NetUtils.IsHost()) SendUserNameServerRpc(CloudManager.instance.GetUserData().userName);
        sliderValue.OnValueChanged += OnSliderValueChange;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnInGameSceneLoaded;
    }

    #endregion


    private void OnInGameSceneLoaded(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        if (sceneName == SceneName.GameScene)
        {
            GameManager.instance.SetSpawner();
            InGameUI.instance.InitAllUserName(hostName, clientName);
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnInGameSceneLoaded;
        }
    }


    private void OnSliderValueChange(float oldValue, float newValue)
    {
        loadingSlider.value = newValue;
    }

    private void OnLoadinStringValueChange(FixedString32Bytes oldValue, FixedString32Bytes newValue)
    {
        loadingText.text = newValue.ToString();
    }


    public override void OnDestroy()
    {
        base.OnDestroy();

        /* if (NetworkManager.Singleton != null)
         {
             if (NetworkManager.Singleton.SceneManager != null)
             {
                 NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnInGameSceneLoaded;
             }
         }*/

        sliderValue.OnValueChanged -= OnSliderValueChange;
    }
}

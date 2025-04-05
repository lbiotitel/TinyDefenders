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
    [SerializeField] private TextMeshProUGUI loadingText;
    private bool hostReady = false; 
    private bool clientReady = false;
    private bool gameStart = false;
    private int loadIndex = 0;
    private string hostName;
    private string clientName;

    private void Awake()
    {
        loadingSlider.value = 0f;
    }

    public void StartLoadingScene()
    {
        if (!NetUtils.IsHost()) SendUserNameServerRpc(CloudManager.instance.GetUserData().userName);
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnInGameSceneLoaded;
        LoadResource();
    }


    private async void LoadResource()
    {
        loadIndex += await SoDataManager.instance.InitDrawHeroCardProbability() == true ? 1 : 0;
        loadIndex += await SoDataManager.instance.InitAllHeroSynergyData() == true ? 1 : 0;
        loadIndex += await SoDataManager.instance.InitAllCombineData() == true ? 1 : 0;
        loadIndex += await SoDataManager.instance.InitInGameQuestData() == true ? 1 : 0;
    }


    private void Update()
    {
        loadingSlider.value = loadIndex * (1f / 4f);
        LoadingTextLogic();
        CheckAllPlayerReady();
    }

    private void CheckAllPlayerReady()
    {
        if (hostReady && clientReady)
        {
            if (NetUtils.GetPlayerNetworkType() == PlayerNetworkType.HOST)
            {
                if (!gameStart)
                {
                    gameStart = true;
                    NetworkManager.SceneManager.LoadScene(SceneName.ForestScene, LoadSceneMode.Single);
                }
            }
        }
    }

    private void LoadingTextLogic()
    {
        switch (loadIndex)
        { 
            case 0:
                loadingText.text = "리소스 로드중...";
                break;
            case 4:
                loadingText.text = "리소스 로드 완료...";
                loadIndex++;
                LoadCompleteServerRpc((int)NetUtils.GetPlayerNetworkType());
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoadCompleteServerRpc(int networkType)
    { 
        PlayerNetworkType playerNetworkType = (PlayerNetworkType)networkType;

        if (playerNetworkType == PlayerNetworkType.HOST)
        {
            hostReady = true;
        }
        else {
            clientReady = true;
        }
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
    }


    #endregion


    private void OnInGameSceneLoaded(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        if (sceneName == SceneName.ForestScene)
        {
            GameManager.instance.SetSpawner();
            GameManager.instance.InitAllUserName(hostName, clientName);
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnInGameSceneLoaded;
        }
    }


}

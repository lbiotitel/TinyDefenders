using System.Collections.Generic;
using TMPro;
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
    private bool hostInfoReady = false;
    private bool clientInfoReady = false;

    private bool gameStart = false;
    private int loadIndex = 0;

    private string hostName;
    private int hostLevel;
    private int hostProfileImageIndex;

    private string clientName;
    private int clientLevel;
    private int clientProfileImageIndex;

    private string mapName;
    private Dictionary<string, bool> ownRelicSODic;

    private void Awake()
    {
        loadingSlider.value = 0f;
    }

    public void StartLoadingScene(string mapName)
    {
        this.mapName = mapName;
        if (NetUtils.GetPlayerNetworkType() == PlayerNetworkType.CLIENT)
        {
            UserData userData = CloudManager.instance.GetUserData();
            SendUserNameServerRpc(userData.userName,userData.userLevel,userData.userProfileImageIndex);
        }
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnInGameSceneLoaded;
        LoadResource();
    }


    private async void LoadResource()
    {
        UserData userData = await CloudManager.instance.LoadUserData();
        ownRelicSODic = userData.relicDataDic;
        loadIndex += await GameResourceManager.instance.InitDrawHeroCardProbability() == true ? 1 : 0;
        loadIndex += await GameResourceManager.instance.InitAllHeroSynergyData() == true ? 1 : 0;
        loadIndex += await GameResourceManager.instance.InitInGameQuestData() == true ? 1 : 0;
        loadIndex += await GameResourceManager.instance.InitAllMonsterData(mapName) == true ? 1 : 0;
        loadIndex += await GameResourceManager.instance.InitIngameRequireExpSO() == true ? 1 : 0;
    }


    private void Update()
    {
        loadingSlider.value = loadIndex * (1f / 5f);
        LoadingTextLogic();
        CheckAllPlayerReady();
    }

    private void CheckAllPlayerReady()
    {
        if (hostReady && clientReady && hostInfoReady && clientInfoReady)
        {
            if (NetUtils.GetPlayerNetworkType() == PlayerNetworkType.HOST)
            {
                if (!gameStart)
                {
                    gameStart = true;
                    NetworkManager.SceneManager.LoadScene(mapName, LoadSceneMode.Single);
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
            case 5:
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
        else
        {
            clientReady = true;
        }
    }


    #region Set Players Info
    //send users name
    [ServerRpc(RequireOwnership = false)]
    private void SendUserNameServerRpc(string clientUserName,int clientUserLevel,int clientProfilImageIndex)
    {
        if (NetUtils.IsHost())
        {
            UserData userData = CloudManager.instance.GetUserData();    
            SendUserNameClientRpc(userData.userName, clientUserName,userData.userLevel,clientUserLevel,userData.userProfileImageIndex,clientProfilImageIndex);
        }
    }

    [ClientRpc]
    private void SendUserNameClientRpc(string hostName, string clientName,int hostLevel, int clientLevel, int hostProfileImageIndex,int clientProfileImageIndex)
    {
        this.hostName = hostName;
        this.clientName = clientName;
        this.hostLevel= hostLevel;
        this.clientLevel = clientLevel;
        this.hostProfileImageIndex = hostProfileImageIndex;
        this.clientProfileImageIndex = clientProfileImageIndex;

        hostInfoReady = true;
        clientInfoReady = true;
    }


    #endregion


    private void OnInGameSceneLoaded(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        if (sceneName == mapName)
        {
            GameManager.instance.SetSpawner();
            GameManager.instance.InitMap(mapName);
            GameManager.instance.InitAllUserInfo(hostName, clientName);
            GameManager.instance.InitRelicDatas(ownRelicSODic);
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnInGameSceneLoaded;
        }
    }

}

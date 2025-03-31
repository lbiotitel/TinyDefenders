using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public partial class NetManager : MonoBehaviour
{
    private const string dEncryption = "dtls";
    private const string wEncryption = "wss";


    #region Friend Matching
    public async Task<bool> JoinGameWithCode(string joinCode)
    {
        if (string.IsNullOrEmpty(joinCode))
        {
#if UNITY_EDITOR
            Debug.Log("유효하지않은 코드입니다.");
#endif
            return false;
        }

        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, dEncryption));

            StartClient();
#if UNITY_EDITOR
            Debug.Log("현재 코드의 방에 접속 완료");
#endif

            return true;
        }
        catch (LobbyServiceException e)
        {
#if UNITY_EDITOR
            Debug.Log("현재 코드의 방에 접속할 수 없습니다." + e);
#endif
            DestroyFriendMatching();
            return false;
        }
    }

    public async Task<string> CreateFriendMatching()
    {
        try
        {

            Allocation allocation = await AllocateRelay();
            string joinCode = await GetRelayJoinCode(allocation);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, dEncryption));

            StartHost();

            return joinCode;

        }
        catch (RelayServiceException e)
        {
#if UNITY_EDITOR
            Debug.Log("친구 매칭 만들기 실패" + e);
#endif
            DestroyFriendMatching();
            return string.Empty;
        }
    }

    public bool DestroyFriendMatching()
    {
        try
        {
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
            return true;
        }
        catch (RelayServiceException e)
        {
#if UNITY_EDITOR
            Debug.Log("친구 매칭 파괴 실패" + e);
#endif
            return false;
        }
    }

    #endregion

    #region About Random Matching

    public async Task<bool> StartRandomMatching()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
#if UNITY_EDITOR
            Debug.Log("로그인이 되지않음");
#endif
            return false;
        }

        currentLobby = await FindAvailableLobby();

        if (currentLobby == null)
        {
            await CreateNewLobby();
        }
        else
        {
            await QuickJoinLobby();
        }

        return true;
    }

    private async Task<Lobby> FindAvailableLobby()
    {
        try
        {
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            if (queryResponse.Results.Count > 0)
            {
                return queryResponse.Results[0];
            }
        }
        catch (LobbyServiceException e)
        {
#if UNITY_EDITOR
            Debug.Log("로비 찾기 실패" + e);
#endif
        }

        return null;
    }

    public async void DestroyLobby()
    {
        try
        {
            if (!string.IsNullOrEmpty(currentLobby.Id))
            {
#if UNITY_EDITOR
                Debug.Log(currentLobby.Id + "로비 파괴");
#endif
                NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
            }
            if (NetUtils.IsHost())
            {
                if (currentLobby == null) return;
                await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
                NetworkManager.Singleton.Shutdown();
            }
        }
        catch (LobbyServiceException e)
        {
#if UNITY_EDITOR
            Debug.Log("로비 파괴 실패" + e);
#endif
        }
    }

    private async Task CreateNewLobby()
    {
        try
        {
            Allocation allocation = await AllocateRelay();
            string joinCode = await GetRelayJoinCode(allocation);

            currentLobby = await LobbyService.Instance.CreateLobbyAsync("랜덤 매칭방", maxPlayers);
#if UNITY_EDITOR
            Debug.Log("로비 생성" + currentLobby.Id);
#endif

            await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                    { LobbyOptionKey.RelayJoinCode.ToString(), new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, dEncryption));

            StartHost();
        }
        catch (LobbyServiceException e)
        {
#if UNITY_EDITOR
            Debug.Log("로비 생성 실패" + e);
#endif
        }
    }

    private async Task QuickJoinLobby()
    {
        try
        {
            currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
#if UNITY_EDITOR
            Debug.Log("로비 참여" + currentLobby.Id);
#endif

            string joinCode = currentLobby.Data[LobbyOptionKey.RelayJoinCode.ToString()].Value;

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, dEncryption));

            StartClient();

        }
        catch (LobbyServiceException e)
        {
#if UNITY_EDITOR
            Debug.Log("로비 참여 실패" + e);
#endif
        }
    }

    #endregion


    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            return allocation;
        }
        catch (LobbyServiceException e)
        {
#if UNITY_EDITOR
            Debug.Log("Relay 서버 할당 실패 " + e);
#endif
            return null;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            return joinCode;

        }
        catch (RelayServiceException e)
        {
#if UNITY_EDITOR
            Debug.Log("Join Code 가져오기 실패" + e);
#endif
            return null;
        }
    }

    private void StartHost()
    {
#if UNITY_EDITOR
        Debug.Log("호스트 시작");
#endif
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        MainSceneUI.instance.ShowMatchingCompleteUI();
        Invoke("DelayClientConnect", 1f);
    }

    private void DelayClientConnect()
    {
        OnPlayerJoined();
#if UNITY_EDITOR
        Debug.Log("클라이언트가 호스트에 연결완료");
#endif
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
        NetworkManager.Singleton.Shutdown();
    }

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
        MainSceneUI.instance.ShowMatchingCompleteUI();
#if UNITY_EDITOR
        Debug.Log("클라이언트 시작");
#endif
    }

    private void OnDestroy()
    {

    }
}

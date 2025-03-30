using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
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
            Debug.Log("유효하지않은 코드입니다.");
            return false;
        }

        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, dEncryption));

            StartClient();
            Debug.Log("현재 코드의 방에 접속 완료");

            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("현재 코드의 방에 접속할 수 없습니다." + e);
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
            Debug.Log("친구 매칭 만들기 실패" + e);
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
            Debug.Log("친구 매칭 파괴 실패" + e);
            return false;
        }
    }

    #endregion

    #region About Random Matching

    public async Task<bool> StartRandomMatching()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("로그인이 되지않음");
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
            Debug.Log("로비 찾기 실패" + e);
        }

        return null;
    }

    public async void DestroyLobby()
    {
        try
        {
            if (!string.IsNullOrEmpty(currentLobby.Id))
            {
                Debug.Log(currentLobby.Id + "로비 파괴");
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
            Debug.Log("로비 파괴 실패" + e);
        }
    }

    private async Task CreateNewLobby()
    {
        try
        {
            Allocation allocation = await AllocateRelay();
            string joinCode = await GetRelayJoinCode(allocation);

            currentLobby = await LobbyService.Instance.CreateLobbyAsync("랜덤 매칭방", maxPlayers);

            Debug.Log("로비 생성" + currentLobby.Id);

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
            Debug.Log("로비 생성 실패" + e);
        }
    }

    private async Task QuickJoinLobby()
    {
        try
        {
            currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            Debug.Log("로비 참여" + currentLobby.Id);

            string joinCode = currentLobby.Data[LobbyOptionKey.RelayJoinCode.ToString()].Value;

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, dEncryption));

            StartClient();

        }
        catch (LobbyServiceException e)
        {
            Debug.Log("로비 참여 실패" + e);

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
            Debug.Log("Relay 서버 할당 실패 " + e);
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
            Debug.Log("Join Code 가져오기 실패" + e);
            return null;
        }
    }

    private void StartHost()
    {
        Debug.Log("호스트 시작");
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
        Debug.Log("클라이언트가 호스트에 연결완료");
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
        Debug.Log("클라이언트 시작");
    }

    private void OnDestroy()
    {

    }
}

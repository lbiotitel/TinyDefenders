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
using UnityEngine;

public partial class NetworkLobbyManager : MonoBehaviour
{

    [SerializeField] private float heartBeatInterval;
    private MapType mapType;
    private Coroutine heartBeatCoroutine = null;

    #region About Network HeartBeat
    private IEnumerator NetworkHeartBeatCoroutine()
    {
        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            yield return new WaitForSeconds(heartBeatInterval);
        }
    }
    #endregion

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

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, EncryptionType.dEncryption));

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

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, EncryptionType.dEncryption));

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
            ReleaseEvent();
            NetworkManager.Singleton.Shutdown();
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

    public async Task<bool> StartRandomMatching(MapType mapType)
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
#if UNITY_EDITOR
            Debug.Log("로그인이 되지않음");
#endif
            return false;
        }
        Debug.Log(mapType);
        currentLobby = await FindAvailableLobby();

        if (currentLobby == null)
        {
            return await CreateNewLobby(mapType);
        }
        else
        {
            return await QuickJoinLobby(mapType);
        }
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

    public async Task<bool> DestroyLobby()
    {
        try
        {
            if (NetworkManager.Singleton == null) return false;
            if (currentLobby == null) return false;
            await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
            if (heartBeatCoroutine != null) StopCoroutine(heartBeatCoroutine);
            currentLobby = null;

            return true;
        }
        catch (LobbyServiceException e)
        {
#if UNITY_EDITOR
            Debug.Log("로비 파괴 실패" + e);
#endif
            return false;
        }
    }

    private async Task<bool> CreateNewLobby(MapType mapType)
    {
        try
        {
            Allocation allocation = await AllocateRelay();
            string joinCode = await GetRelayJoinCode(allocation);

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(LobbyInfo.LobbyName + mapType.ToString(), maxPlayers);
#if UNITY_EDITOR
            Debug.Log("로비 생성" + currentLobby.Id);
#endif

            await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                    { LobbyOptionKey.RelayJoinCode.ToString(), new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, EncryptionType.dEncryption));

            heartBeatCoroutine = StartCoroutine(NetworkHeartBeatCoroutine());

            this.mapType = mapType;
            
            StartHost();

            return true;
        }
        catch (LobbyServiceException e)
        {
#if UNITY_EDITOR
            Debug.Log("로비 생성 실패" + e);
#endif
            return false;
        }
    }

    private async Task<bool> QuickJoinLobby(MapType mapType)
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            options.Filter = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.Name,
                    op: QueryFilter.OpOptions.EQ,
                    value: LobbyInfo.LobbyName + mapType.ToString())
            };

            currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
#if UNITY_EDITOR
            Debug.Log("로비 참여" + currentLobby.Id);
#endif

            string joinCode = currentLobby.Data[LobbyOptionKey.RelayJoinCode.ToString()].Value;

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, EncryptionType.dEncryption));

            this.mapType = mapType;

            StartClient();

            return true;
        }
        catch (LobbyServiceException e)
        {
#if UNITY_EDITOR
            Debug.Log("로비 참여 실패" + e);
#endif
            ReleaseEvent();
            NetworkManager.Singleton.Shutdown();
            return false;
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


    private async void OnClientConnected(ulong clientId)
    {
        MainSceneUI.instance.ShowMatchingCompleteUI();
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
        if (NetUtils.IsHost())
        {
            await DestroyLobby();
            Invoke("DelayClientConnect", 2f);
        }
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
        if (NetUtils.GetPlayerNetworkType() == PlayerNetworkType.HOST)
        {
            NetworkSceneManager.instance.ShowNetworkMessage(NetworkMessageType.CLIENT_DISCONNECT);
        }
        else
        {
            NetworkSceneManager.instance.ShowNetworkMessage(NetworkMessageType.HOST_SHUT_DOWN);
        }

        ReleaseEvent();
        NetworkManager.Singleton.Shutdown();
    }

    private void StartHost()
    {
#if UNITY_EDITOR
        Debug.Log("호스트 시작");
#endif
        ReleaseEvent();
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }


    private void StartClient()
    {
        ReleaseEvent();
        NetworkManager.Singleton.StartClient();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
#if UNITY_EDITOR
        Debug.Log("클라이언트 시작");
#endif
    }


    public void ReleaseEvent()
    {
        if (NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.SceneManager != null) NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }


}

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
    public async void JoinGameWithCode(string joinCode)
    {
        if (string.IsNullOrEmpty(joinCode))
        {
            Debug.Log("유효하지않은 코드입니다.");
            return;
        }

        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.HostConnectionData
                );

            StartClient();
            Debug.Log("현재 코드의 방에 접속 완료");

        }
        catch (LobbyServiceException e)
        {
            Debug.Log("현재 코드의 방에 접속할 수 없습니다." + e);
        }
    }

    public async void StartMatching()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("로그인이 되지않음");
            return;
        }

        matchingObject.SetActive(true);

        currentLobby = await FindAvailableLobby();

        if (currentLobby == null)
        {
            await CreateNewLobby();
        }
        else
        {
            await JoinLobby(currentLobby.Id);
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
            Debug.Log("로비 찾기 실패" + e);
        }

        return null;
    }

    private async void DestroyLobby(string lobbyId)
    {
        try
        {
            if (!string.IsNullOrEmpty(lobbyId))
            { 
                await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
                Debug.Log(lobbyId + "로비 파괴");
            }
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown();
                matchingObject.SetActive(false);
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
            currentLobby = await LobbyService.Instance.CreateLobbyAsync("랜덤 매칭방", maxPlayers);
            Debug.Log("로비 생성" + currentLobby.Id);
            await AllocateRelayServerAndJoin(currentLobby);
            cancleButton.onClick.AddListener(() =>
            {
                DestroyLobby(currentLobby.Id);
            });
            StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("로비 생성 실패" + e);
        }
    }

    private async Task JoinLobby(string lobbyId)
    {
        try
        {
            currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            Debug.Log("로비 참여" + currentLobby.Id);
            StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("로비 참여 실패" + e);
        }
    }

    private async Task AllocateRelayServerAndJoin(Lobby lobby)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(lobby.MaxPlayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Relay 서버 할당 Join Code : " + joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Relay 서버 할당 실패 " + e);
        }
    }

    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("호스트 시작");

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnHostDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log("클라이언트가 호스트에 연결완료");
        OnPlayerJoined();
    }

    private void OnHostDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.IsHost)
        { 
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnHostDisconnected;
        }
    }

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("클라이언트 시작");
    }

}

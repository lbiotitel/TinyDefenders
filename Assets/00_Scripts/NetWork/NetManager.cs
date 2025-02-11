using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class NetManager : MonoBehaviour
{

    //Lobby -> 원하는 게임을 찾거나 새 게임을 만들고 대기할 수 있다.
    //Relay -> 매칭된 플레이어들의 relay의 join code로 연결되어, 호스트-클라이언트 방식으로 실시간 멀티환경을 유지
    private Lobby currentLobby;

    [SerializeField] private Button startMatchingButton;


    private async void Start()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        startMatchingButton.onClick.AddListener(() => StartMatching());
    }

    public async void StartMatching()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("로그인이 되지않음");
            return;
        }

        currentLobby = await FindAvailableLobby();

        if (currentLobby == null)
        {
            await CreateNewLobby();
        }
        else {
            await JoinLobby(currentLobby.Id);
        }
    }

    private async Task<Lobby> FindAvailableLobby()
    {
        try {
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            if (queryResponse.Results.Count > 0)
            {
                return queryResponse.Results[0];
            }
        } catch (LobbyServiceException e)
        {
            Debug.Log("로비 찾기 실패" + e);
        }

        return null;
    }

    private async Task CreateNewLobby()
    {
        try
        {
            currentLobby = await LobbyService.Instance.CreateLobbyAsync("랜덤 매칭방", 2);
            Debug.Log("로비 생성" + currentLobby.Id);
            await AllocateRelayServerAndJoin(currentLobby);
            StartHost();
        }
        catch (LobbyServiceException e) {
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
    }

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("클라이언트 시작");
    }
}

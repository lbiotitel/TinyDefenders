using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public partial class NetManager : MonoBehaviour
{

    //Lobby -> 원하는 게임을 찾거나 새 게임을 만들고 대기할 수 있다.
    //Relay -> 매칭된 플레이어들의 relay의 join code로 연결되어, 호스트-클라이언트 방식으로 실시간 멀티환경을 유지
    private Lobby currentLobby;

    private const int maxPlayers = 2;
    [SerializeField] private string gamePlaySceneName = "GamePlayScene";
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

}

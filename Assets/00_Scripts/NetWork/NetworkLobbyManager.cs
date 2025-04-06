using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public partial class NetworkLobbyManager : MonoBehaviour
{
    public static NetworkLobbyManager instance { get; private set; }
    private Lobby currentLobby = null;
    private const int maxPlayers = 2;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { 
            Destroy(gameObject);
        }
    }



    public async Task SingInUnityService()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            await UnityServices.InitializeAsync();
        }
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private async void OnApplicationQuit()
    {
        if (instance != null)
        {
            ReleaseEvent();
            await DestroyLobby();
            if (NetworkManager.Singleton != null) 
                NetworkManager.Singleton.Shutdown();
            instance = null;
        }
    }
}

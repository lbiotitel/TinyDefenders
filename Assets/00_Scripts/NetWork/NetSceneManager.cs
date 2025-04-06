using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class NetworkLobbyManager : MonoBehaviour
{

    private void OnPlayerJoined()
    {
#if UnityEidtor
        Debug.Log("연결된 사람 수" + NetworkManager.Singleton.ConnectedClients.Count);
#endif
        if (NetworkManager.Singleton.ConnectedClients.Count >= maxPlayers)
        {
            ChangeSceneForAllPlayers();
        }
    }

    private void ChangeSceneForAllPlayers()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                SceneName.LoadingScene,
                UnityEngine.SceneManagement.LoadSceneMode.Single
            );
        }
    }

    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        if (sceneName == SceneName.LoadingScene)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
            LoadingScene loadingScene = FindFirstObjectByType<LoadingScene>();
            loadingScene.StartLoadingScene();
        }
    }
}

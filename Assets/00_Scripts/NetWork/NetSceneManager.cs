using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class NetManager : MonoBehaviour
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
                SceneName.GameScene,
                UnityEngine.SceneManagement.LoadSceneMode.Single
                );
        }
    }

    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        if (sceneName == SceneName.GameScene)
        {
            Debug.Log("호출");
            GameManager.instance.SetSpawner();

            if (!NetUtils.IsHost())
            {
                GameManager.instance.SendUserNameServerRpc(CloudManager.instance.userData.userName);
            }

            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
        }
    }

}

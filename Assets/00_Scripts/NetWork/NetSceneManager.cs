using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class NetManager : MonoBehaviour
{

    private void OnPlayerJoined()
    {
        Debug.Log("연결된 사람 수" + NetworkManager.Singleton.ConnectedClients.Count);
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
            GameManager.instance.SetSpawner();

            if (!NetUtils.IsServer())
            {
                GameManager.instance.SendUserNameServerRpc(CloudManager.instance.userData.userName);
            }

            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
        }
    }

}

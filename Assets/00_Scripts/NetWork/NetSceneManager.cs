using Unity.Netcode;
using UnityEngine;

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
            Debug.Log("씬이 넘어가유");
            NetworkManager.Singleton.SceneManager.LoadScene(
                gamePlaySceneName,
                UnityEngine.SceneManagement.LoadSceneMode.Single
                );
        }
    }

}

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
            Debug.Log("씬이 넘어가유");
            NetworkManager.Singleton.SceneManager.LoadScene(
                gamePlaySceneName,
                UnityEngine.SceneManagement.LoadSceneMode.Single
                );
        }
    }

    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        GameObject go = GameObject.Find("GameManager");
        if (go != null)
        {
            GameManager gameManager = go.GetComponent<GameManager>();
            if (NetUtils.IsServer())
            {
                gameManager.SetSpawner();
            }
            else
            {
                gameManager.SendUserNameServerRpc(CloudManager.instance.userData.userName);
            }
        }
    }

}

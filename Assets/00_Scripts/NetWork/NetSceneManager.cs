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

    /*
    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode mode)
{
    if (sceneName == "NextScene") // GameManager가 있는 씬 이름
    {
        GameObject go = GameObject.Find("GameManager");
        if (go == null)
        {
            Debug.LogError("다음 씬에서도 GameManager를 찾을 수 없습니다.");
            return;
        }

        GameManager gameManager = go.GetComponent<GameManager>();
        gameManager.SetSpawner();

        if (!NetUtils.IsServer())
        {
            gameManager.SendUserNameServerRpc(CloudManager.instance.userData.userName);
        }
    }

    NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
}

     * 
     * 
     * 
     */

    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        GameObject go = GameObject.Find("GameManager");
        if (go != null)
        {
            GameManager gameManager = go.GetComponent<GameManager>();
            if (!NetUtils.IsServer())
            {
                gameManager.SendUserNameServerRpc(CloudManager.instance.userData.userName);
            }

            gameManager.SetSpawner();
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
        }
    }


}

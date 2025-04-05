using UnityEngine;


public class NetworkSceneManager : MonoBehaviour
{
    public static NetworkSceneManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private GameObject hostShutDownMessage;
    [SerializeField] private GameObject clientDisconnectMessage;

    public void ShowNetworkMessage(NetworkMessageType networkMessageType)
    {
        switch (networkMessageType)
        {
            case NetworkMessageType.HOST_SHUT_DOWN:
                hostShutDownMessage.SetActive(true);
                break;
            case NetworkMessageType.CLIENT_DISCONNECT:
                clientDisconnectMessage.SetActive(true);
                break;
        }
    }



    private void OnApplicationQuit()
    {
        if (instance != null)
            instance = null;

    }
}

using UnityEngine;

public class DisableNetweorkObject : MonoBehaviour
{



    private void Awake()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Destroy(gameObject);
        }
    }


}

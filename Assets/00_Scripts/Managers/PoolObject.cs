using UnityEngine;

public class PoolObject : MonoBehaviour
{

    private void OnDisable()
    {
        if (PoolingManager.instance != null)
        { 
            PoolingManager.instance.ReturnObject(this.gameObject);
        }
    }
}

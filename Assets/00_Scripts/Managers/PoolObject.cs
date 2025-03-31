using UnityEngine;

public class PoolObject : MonoBehaviour
{


    private void OnDisable()
    {
        PoolingManager.instance.ReturnObject(gameObject);
    }

   
}

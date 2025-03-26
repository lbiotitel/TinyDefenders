using UnityEngine;

public class PoolParticleObject : MonoBehaviour
{

    private void OnParticleSystemStopped()
    {
        if (PoolingManager.instance != null)
        {
            PoolingManager.instance.ReturnObject(this.gameObject);
        }
    }
}

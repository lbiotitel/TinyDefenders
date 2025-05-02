using System.Collections;
using UnityEngine;

public class SpinMotion : ProjectileMotionBase
{ 

    protected override IEnumerator CoProjectileMotion()
    {
        float elapsedTime = 0f;
        float duration = _projectileData.LifeTime; 
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.rotation = Quaternion.Euler(0f, 0f, _projectileData.Speed);

            DamageCallback?.Invoke();
            yield return null;
        }
        DisableObject(0f);
    }
}

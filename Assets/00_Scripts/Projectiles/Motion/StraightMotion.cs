using System.Collections;
using UnityEngine;

public class StraightMotion : ProjectileMotionBase
{
    protected override IEnumerator CoProjectileMotion()
    {
        float length = Vector3.Distance(StartPos, EndPos);
        float totalTime = length / _projectileData.Speed;
        float elapsedTime = 0f;

        while (elapsedTime < totalTime)
        { 
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / totalTime;  
            Vector3 newPos = Vector3.Lerp(StartPos, EndPos, t);
            transform.position = newPos;

            ProjectileRotationLogic(EndPos - StartPos);

            yield return null;
        }

        transform.position = EndPos;
        EndCallback?.Invoke();  
        DisableObject(_projectileData.LifeTime);
    }
}

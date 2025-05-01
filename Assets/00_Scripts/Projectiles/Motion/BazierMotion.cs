using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazierMotion : ProjectileMotionBase
{
    private float HeightArc = 0.5f;

    protected override IEnumerator CoProjectileMotion()
    {
        float length = Vector2.Distance(StartPos, EndPos);
        float totalTime = length /  _projectileData.Speed;
        float elapsedTime = 0.0f;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / totalTime;

            float x = Mathf.Lerp(StartPos.x, EndPos.x, t);
            float baseY = Mathf.Lerp(StartPos.y, EndPos.y, t);
            float arc = HeightArc * Mathf.Sin(t * Mathf.PI);

            float y = baseY + arc;

            Vector3 nextPos = new Vector3(x, y);

            ProjectileRotationLogic(nextPos - transform.position);

            transform.position = nextPos;
            
            yield return null;
        }

        
        EndCallback?.Invoke();
        DisableObject(_projectileData.LifeTime);
    }
}

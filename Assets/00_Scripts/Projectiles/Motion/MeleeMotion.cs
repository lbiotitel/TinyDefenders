using System;
using System.Collections;
using UnityEngine;

public class MeleeMotion : ProjectileMotionBase
{

    public void Init(Vector3 endPos, ProjectileSO projectileData, Action endCallback = null)
    {
        EndPos = endPos;
        _projectileData = projectileData;

        EndCallback -= endCallback;
        EndCallback += endCallback;

        if (_coProjectileMotion != null)
            StopCoroutine(_coProjectileMotion);

        _coProjectileMotion = StartCoroutine(CoProjectileMotion());
    }


    protected override IEnumerator CoProjectileMotion()
    {
        yield return null;

        transform.position = EndPos;

        EndCallback?.Invoke();
        DisableObject(_projectileData.LifeTime);
    }
}

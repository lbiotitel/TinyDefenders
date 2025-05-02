using System;
using System.Collections;
using UnityEngine;

public class LaserMotion : ProjectileMotionBase
{

    private LineRenderer _lineRenderer;
    public new void Init(Vector3 startPos,Vector3 endPos, ProjectileSO projectileData ,Action endCallback = null)
    {
        TryGetComponent<LineRenderer>(out _lineRenderer);

        Debug.Log(_lineRenderer);

        StartPos = startPos;
        EndPos = endPos;
        _projectileData = projectileData;

        DamageCallback -= endCallback;
        DamageCallback += endCallback;

        if (_coProjectileMotion != null)
            StopCoroutine(_coProjectileMotion);

        _coProjectileMotion = StartCoroutine(CoProjectileMotion());
    }

    protected override IEnumerator CoProjectileMotion()
    {
        _lineRenderer.SetPosition(0,StartPos);
        _lineRenderer.SetPosition(1, EndPos);
        
        yield return new WaitForSeconds(_projectileData.LifeTime);

        DamageCallback?.Invoke();
        DisableObject(0f);
    }
}

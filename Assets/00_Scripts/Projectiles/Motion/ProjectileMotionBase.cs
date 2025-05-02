using System;
using System.Collections;
using UnityEngine;

public abstract class ProjectileMotionBase : MonoBehaviour
{
    public Vector3 StartPos { get; protected set; }
    public Vector3 EndPos { get; protected set; }
    protected Coroutine _coProjectileMotion;
    protected ProjectileSO _projectileData;
    protected ParticleSystem _particleSystem;
    protected SpriteRenderer _spriteRenderer;
    public Action DamageCallback { get; set; }
    public void Init(Vector3 startPos,Vector3 endPos,ProjectileSO projectileData,Action endCallback = null)
    { 
        
        StartPos = startPos;
        EndPos = endPos;
        _projectileData = projectileData;

        DamageCallback -= endCallback; 
        DamageCallback += endCallback;

        TryGetComponent<ParticleSystem>(out _particleSystem);
        TryGetComponent<SpriteRenderer>(out _spriteRenderer);

        if (_particleSystem != null)
            _particleSystem.Play();
        if (_spriteRenderer != null)
            _spriteRenderer.enabled = true;
        if (_coProjectileMotion != null)
            StopCoroutine(_coProjectileMotion);

        _coProjectileMotion = StartCoroutine(CoProjectileMotion());
    }


    protected void ProjectileRotationLogic(Vector3 dir)
    {
        switch (_projectileData.ProjectileType)
        {
            case ProjectileType.LOOKAT:
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
                break;
            case ProjectileType.ROTATE:
                transform.Rotate(0,0,5f);
                break;
        }

    }

    protected void DisableObject(float time)
    {
        if (_particleSystem != null)
            _particleSystem.Stop();
        if (_spriteRenderer != null)
            _spriteRenderer.enabled = false;
        StartCoroutine(DisableObjectCoroutine(time));
    }

    private IEnumerator DisableObjectCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        if (PoolingManager.instance != null)
        {
            PoolingManager.instance.ReturnObject(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_coProjectileMotion != null)
            StopCoroutine(_coProjectileMotion);
        _coProjectileMotion = null;
        DamageCallback = null;
    }


    protected abstract IEnumerator CoProjectileMotion();
}

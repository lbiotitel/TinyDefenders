using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Rendering;

public class BaseProjectile : MonoBehaviour
{
    [Header("Base Projectile Value")]
    [SerializeField] protected float speed;
    [SerializeField] protected ProjectileType projectileType = ProjectileType.NORMAL;
    [SerializeField ] protected ProjectileMoveType moveType = ProjectileMoveType.NORMAL;
    [SerializeField][Range(0f,10f)] protected float lifeTime = 0.2f;
    [SerializeField] protected GameObject destroyParticlePrefab;
    [SerializeField] protected string projectileSFX;
    
    protected GameObject destroyParticle;
    protected Hero referenceHero;
    protected Monster target;
    protected bool isAttack = false;
    protected double damage;
    protected bool isReferenceHeroExist = true;    

    private List<DebuffEffect> debuffEffects = new List<DebuffEffect>();

    public virtual void Init(Monster target, Hero referenceHero,double damage,List<DebuffEffect> debuffEffects = null)
    {
        SoundManager.instance.PlaySfxSound(projectileSFX,SFXTYPE.PROJECTILE);

        this.referenceHero = referenceHero;
        this.target = target;
        this.debuffEffects = debuffEffects;
        this.damage = damage;

        ProjectileMoveLogic();
    }


    protected virtual void Update()
    {
        if (target == null) return;
        if (referenceHero == null)
        {
            if (isReferenceHeroExist)
            {
                isReferenceHeroExist = false;
                DisableObject(0.1f);
            }
        }
    }

    protected void ProjectileTypeLogic()
    {
        switch (projectileType)
        {
            case ProjectileType.NONE:
                break;
            case ProjectileType.NORMAL:
                RotateToForwardDirection();
                break;
            case ProjectileType.ROTATE:
                SpinningProjectile();
                break;
        }
    }

    private void SpinningProjectile()
    {
        transform.Rotate(0, 0, speed);
    }

    private void RotateToForwardDirection()
    {
        Vector2 direction = target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected virtual void ProjectileMoveLogic()
    {
        switch (moveType)
        {
            case ProjectileMoveType.NORMAL:
                StartCoroutine(ProjectileStraightMoveLogic());
                break;
            case ProjectileMoveType.BAZIER:
                StartCoroutine(ProjectileBazierMoveLogic());
                break;
        }
    }

    private IEnumerator ProjectileStraightMoveLogic()
    {
        while (true)
        {
            if(target == null) break;
            ProjectileTypeLogic();
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position,
                    target.transform.position, speed * Time.deltaTime);
            }
            else
                break;

            yield return null;  
        }

        HitMonster();
    }

    private IEnumerator ProjectileBazierMoveLogic()
    {
        Vector3 startPos = transform.position;

        Vector3 midPoint = (startPos + target.transform.position) / 2f;
        midPoint.y += Random.Range(-1f, 1f);


        float elapsedTime = 0.0f;

        while (elapsedTime < 1f)
        {
            if (target == null) break;
            ProjectileTypeLogic();

            float t = elapsedTime / 1f;

            Vector3 curvePoint = MathUtils.GenerateBezierPoint(t, startPos,
                midPoint, target.transform.position);

            transform.position = new Vector3(curvePoint.x, curvePoint.y, 0);

            elapsedTime += Time.deltaTime * speed;
            yield return null;
        }

        HitMonster();
    }


    protected virtual void HitMonster()
    {
        if (target == null)
            return;
        if (!isAttack)
        {
            isAttack = true;

            if(destroyParticlePrefab != null)
                destroyParticle = PoolingManager.instance.GetObject(destroyParticlePrefab, transform.position);

            AttackMonster(target.gameObject);

            DisableObject(lifeTime);
        }
    }

    protected void AttackMonster(GameObject monster)
    {
        if (NetUtils.IsHost() && monster != null && !monster.GetComponent<Monster>().IsDead())
        {
            NetworkRpcManager.instance.AttackMonster(
                referenceHero.heroOwnerNetworkType,
                damage,
                referenceHero,
                monster.GetComponent<Monster>(),
                debuffEffects == null ? referenceHero.GetHeroData().debuffEffects : debuffEffects);
        }
    }


    protected void DisableObject(float time)
    {
        StartCoroutine(DisableObjectCoroutine(lifeTime));
    }

    private IEnumerator DisableObjectCoroutine(float time)
    { 
        yield return new WaitForSeconds(time);

        target = null;
        referenceHero = null;
        isAttack = false;
        debuffEffects = null;


        if (PoolingManager.instance != null)
        {
            if (destroyParticlePrefab != null)
                PoolingManager.instance.ReturnObject(destroyParticle);
            PoolingManager.instance.ReturnObject(gameObject);
        }
    }

}

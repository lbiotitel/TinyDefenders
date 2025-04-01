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
    [SerializeField][Range(0f,10f)] protected float lifeTime = 0.2f;
    [SerializeField] protected GameObject destroyParticlePrefab;
    
    protected GameObject destroyParticle;
    protected Hero referenceHero;
    protected Transform target;
    protected bool isAttack = false;
    protected double damage;

    private List<DebuffEffect> debuffEffects = new List<DebuffEffect>();

    public virtual void Init(Transform target, Hero referenceHero,double damage,List<DebuffEffect> debuffEffects = null)
    {
        this.referenceHero = referenceHero;
        this.target = target;
        this.debuffEffects = debuffEffects;
        this.damage = damage;
    }


    protected virtual void Update()
    {
        if (target == null) return;
        ProjectileLogic();
    }

    protected virtual void ProjectileLogic()
    {
        ProjectileMoveLogic();
        ProjectileTypeLogic();
    }

    protected void ProjectileTypeLogic()
    {
        switch (projectileType)
        {
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
        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected virtual void ProjectileMoveLogic()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        if (distance > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position,
                target.position, speed * Time.deltaTime);
        }
        else
        {
            HitMonster();
        }
    }

    protected virtual void HitMonster()
    {
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

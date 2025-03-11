using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.Rendering;

public class BaseProjectile : MonoBehaviour
{
    [Header("Base Projectile Value")]
    [SerializeField] protected float speed;
    [SerializeField] protected ProjectileType projectileType = ProjectileType.NORMAL;
    [SerializeField][Range(0.2f,1f)] protected float lifeTime = 0.2f;
    protected GameObject destroyParticle;
    protected Hero referenceHero;
    protected Transform target;
    protected bool isAttack = false;

    public virtual void Init(Transform target, Hero referenceHero)
    {
        this.referenceHero = referenceHero;
        this.target = target;
    }


    protected virtual void Update()
    {
        if (target == null)
        {
            StopAllCoroutines();
            HitMonster();
            return;
        }
        ProjectileLogic();
    }

    protected virtual void ProjectileLogic()
    {
        ChasingTarget();
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

    protected virtual void ChasingTarget()
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

            destroyParticle = PoolingManager.instance.GetObject("MonsterHitParticle", transform.position);

            if (NetUtils.IsServer() && target != null && !target.GetComponent<Monster>().IsDead())
                referenceHero.AttackMonster(target.GetComponent<Monster>());

            Destroy(this.gameObject, lifeTime);
        }
    }

    protected virtual void OnDestroy()
    {
        if (destroyParticle != null)
        {
            PoolingManager.instance.ReturnObject(destroyParticle);
        }
    }
}

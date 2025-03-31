using System.Threading;
using UnityEngine;

public class SpellCardBaseProjectile : MonoBehaviour
{
    [Header("Base Card Projectile Value")]
    [SerializeField] protected float speed;
    [SerializeField] protected ProjectileType projectileType = ProjectileType.NORMAL;
    [SerializeField][Range(0.2f, 10f)] protected float lifeTime = 0.2f;

    protected GameObject destroyParticle;
    protected AttackCardSO referenceCard;
    protected Transform target;
    protected bool isAttack = false;

    public virtual void Init(Transform target, AttackCardSO referenceCard)
    {
        this.referenceCard = referenceCard;
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


    //상속 받아 구현
    protected virtual void HitMonster() { }

    protected void AttackMonster(GameObject monster,double damage)
    {
        if (NetUtils.IsHost() && monster != null && !monster.GetComponent<Monster>().IsDead())
        {
            NetworkRpcManager.instance.AttackMonster(NetUtils.GetPlayerNetworkType(), damage,null,monster.GetComponent<Monster>(),
                referenceCard.debuffEffects);
        }
    } 

    protected virtual void OnDestroy()
    {
        if (destroyParticle != null)
        {
            PoolingManager.instance.ReturnObject(destroyParticle);
            Destroy(target.gameObject);
        }
    }
}

using UnityEngine;
using UnityEngine.Rendering;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject destroyParticle;
    [SerializeField] private ProjectileType projectileType = ProjectileType.NORMAL;
    private Hero referenceHero;
    private Transform target;

    public void Init(Transform target,Hero referenceHero)
    {
        this.referenceHero = referenceHero;
        this.target = target;
    }

    private void Update()
    {
        ChasingTarget();
        ProjectileTypeLogic();
    }

    private void ProjectileTypeLogic()
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
        transform.Rotate(0,0,speed);
    }

    private void RotateToForwardDirection()
    {
        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,angle);
    }

    private void ChasingTarget()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        if (distance > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position,
                target.position, speed * Time.deltaTime);
        }
        else
        {
            Instantiate(destroyParticle,transform.position,Quaternion.identity);
            referenceHero.AttackMonster();
            Destroy(this.gameObject);
        }
    }
}

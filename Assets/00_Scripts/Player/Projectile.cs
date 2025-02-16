using UnityEngine;
using UnityEngine.Rendering;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject destroyParticle;

    private Transform target;
    private int damage;

    public void Init(Transform target,int damage)
    {
        this.target = target;
        this.damage = damage;
    }

    private void Update()
    {
        ChasingTarget();
        RotateToForwardDirection();
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
            //target.GetComponent<Monster>().GetDamage(damage);
            Destroy(this.gameObject);
        }
    }
}

using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.Rendering;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject destroyParticle;
    [SerializeField] private ProjectileType projectileType = ProjectileType.NORMAL;
    [SerializeField] private bool isBezier = false;
    private Hero referenceHero;
    private Transform target;

    public void Init(Transform target, Hero referenceHero)
    {
        this.referenceHero = referenceHero;
        this.target = target;
    }


    private void Start()
    {
        if(isBezier) StartCoroutine(ChasingTargetBazier());
    }

    private void Update()
    {
        if (target == null)
        {
            StopAllCoroutines();
            HitMonster();
            return;
        }
        if (!isBezier)
        {
            ChasingTarget();
        }
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
        transform.Rotate(0, 0, speed);
    }

    private void RotateToForwardDirection()
    {
        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }


    private IEnumerator ChasingTargetBazier()
    { 
        Vector3 startPos = transform.position;

        Vector3 midPoint = (startPos + target.position) / 2f;
        midPoint.y += Random.Range(-1f,1f);


        float elapsedTime = 0.0f;
        float divSpeed = speed / 20f;
        while (elapsedTime < divSpeed)
        {
            if (target == null) break;

            float t = elapsedTime / divSpeed;
         
            Vector3 curvePoint = MathUtils.GenerateBezierPoint(t, startPos,
                midPoint, target.position);

            transform.position = new Vector3(curvePoint.x, curvePoint.y, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        HitMonster();
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
            HitMonster();
        }
    }

    private void HitMonster()
    {
        Instantiate(destroyParticle, transform.position, Quaternion.identity);
        Destroy(this.gameObject);

        if (NetUtils.IsServer() && target != null)
            referenceHero.AttackMonster();
    }
}

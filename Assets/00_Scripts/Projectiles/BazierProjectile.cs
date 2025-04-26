using System.Collections;
using UnityEngine;

public class BazierProjectile : BaseProjectile
{


    protected override void ProjectileLogic()
    {
        StartCoroutine(ChasingTargetBazier());
        ProjectileTypeLogic();
    }

    private IEnumerator ChasingTargetBazier()
    {
        Vector3 startPos = transform.position;

        Vector3 midPoint = (startPos + target.position) / 2f;
        midPoint.y += Random.Range(-1f, 1f);


        float elapsedTime = 0.0f;

        while (elapsedTime < 1f)
        {
            if (target == null) break;

            float t = elapsedTime / 1f;

            Vector3 curvePoint = MathUtils.GenerateBezierPoint(t, startPos,
                midPoint, target.position);

            transform.position = new Vector3(curvePoint.x, curvePoint.y, 0);

            elapsedTime += Time.deltaTime * speed;
            yield return null;
        }

        HitMonster();
    }
}

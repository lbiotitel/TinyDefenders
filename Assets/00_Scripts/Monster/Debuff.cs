using System.Collections;
using UnityEngine;

public partial class Monster : Entity
{
    private float originSpeed;
    private Coroutine slowCoroutine;
    [Header("Slow Effect")]
    [SerializeField] private Color slowColor; 

    public void SlowDebuff(float slowAmount, float slowDuration)
    {
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
        slowCoroutine = StartCoroutine(SlowDebuffCoroutine(slowAmount, slowDuration));
    }

    private IEnumerator SlowDebuffCoroutine(float slowAmount, float slowDuration)
    {
        originSpeed = speed;
        float newSpeed = originSpeed - (originSpeed * slowAmount);
        newSpeed = Mathf.Max(newSpeed, 0.1f);
        speed = newSpeed;
        spriteRenderer.color = slowColor;
        yield return new WaitForSeconds(slowDuration);

        speed = originSpeed;
        spriteRenderer.color = Color.white;
    }

}

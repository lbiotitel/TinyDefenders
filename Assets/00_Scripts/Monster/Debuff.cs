using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

//Monster Dubuff
public partial class Monster : Entity
{

    [Header("Slow Effect")]
    [SerializeField] private Color slowColor;
    private Coroutine slowCoroutine;
    private int currentSlowDebuffLevel = 0;
    private float originalSpeed;

    [Header("Stun Effect")]
    private GameObject stunEffect;
    private Coroutine stunCoroutine;
    private bool isStun = false;


    [ServerRpc(RequireOwnership = false)]
    public void DebuffServerRpc(int debuffType, float[] values)
    {
        DebuffType debuff = (DebuffType)debuffType;

        switch (debuff)
        {
            case DebuffType.SLOW:
                {
                    int debuffLevel = (int)values[0];
                    float slowAmount = values[1];
                    float slowDuration = values[2];

                    if (currentSlowDebuffLevel >= debuffLevel) return;
                    currentSlowDebuffLevel = debuffLevel;

                    SlowDebuffClientRpc(slowAmount, slowDuration);
                }
                break;

            case DebuffType.STUN:
                {
                    float stunDuration = (float)values[0];
                    StunDebuffClientRpc(stunDuration);
                }
                break;
        }
    }

    private void CheckAndStopCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
    }

    [ClientRpc]
    private void StunDebuffClientRpc(float stunDuration)
    {
        CheckAndStopCoroutine(stunCoroutine);
        stunCoroutine = StartCoroutine(DebuffCoroutine(stunDuration,
            () =>
            {
                isStun = true;
            }, () =>
            {
                isStun = false;
            }));
    }

    [ClientRpc]
    private void SlowDebuffClientRpc(float slowAmount, float slowDuration)
    {
        CheckAndStopCoroutine(slowCoroutine);

        slowCoroutine = StartCoroutine(
          DebuffCoroutine(
              slowDuration,
              () =>
              {
                  float newSpeed = originalSpeed - (originalSpeed * slowAmount);
                  newSpeed = Mathf.Max(newSpeed, 0.1f);
                  speed = newSpeed;
                  spriteRenderer.color = slowColor;
              },
              () =>
              {
                  speed = originalSpeed;
                  spriteRenderer.color = Color.white;
              }
          )
      );
    }



    private IEnumerator DebuffCoroutine(float debuffDuration, Action firstAction, Action secondAction)
    {
        firstAction?.Invoke();
        yield return new WaitForSeconds(debuffDuration);
        secondAction?.Invoke();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}

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
    [SerializeField]private GameObject stunEffect;
    private Coroutine stunCoroutine;
    private bool isStun = false;

    [Header("Bleed Effect")]
    [SerializeField] private GameObject bleedEffect;
    private Coroutine bleedCoroutine;
    private float bleedPercent;


    [ServerRpc(RequireOwnership = false)]
    public void DebuffServerRpc(int debuffType, float[] values,bool isServer)
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
            case DebuffType.BLEED:
                {
                    if (bleedCoroutine != null) return;
                    float bleedDuration = (float)values[0];
                    float bleedPercent = (float)values[1];
                    BleedDebuffClientRpc(bleedPercent,bleedDuration,isServer);
                }
                break;
        }
    }

    private void CheckAndStopCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            if (isDead) return;
            StopCoroutine(coroutine);
        }
    }

    [ClientRpc]
    private void StunDebuffClientRpc(float stunDuration)
    {
        CheckAndStopCoroutine(stunCoroutine);
        stunCoroutine = StartCoroutine(DebuffCoroutine(stunDuration,
            () =>
            {
                if (!stunEffect.activeSelf)
                {
                    stunEffect.gameObject.SetActive(true);
                } 
                isStun = true;
            }, () =>
            {
                stunEffect.gameObject.SetActive(false);
                isStun = false;
            }));
    }

    [ClientRpc]
    private void BleedDebuffClientRpc(float bleedPercent, float bleedDuration,bool isServer)
    {
        CheckAndStopCoroutine(bleedCoroutine);
        Coroutine bleedDamageCoroutine = null;
        bleedCoroutine = StartCoroutine(
          DebuffCoroutine(
              bleedDuration,
              () =>
              {
                  bleedEffect.gameObject.SetActive(true);
                  bleedDamageCoroutine = StartCoroutine(BleedDamageCoroutine(isServer));
              },
              () =>
              {
                  bleedEffect.gameObject.SetActive(false);
                  StopCoroutine(bleedDamageCoroutine);
                  bleedCoroutine = null;
              }
          )
      );
    }

    private IEnumerator BleedDamageCoroutine(bool isServer)
    {
        while (true)
        {
            float bleedHp = Mathf.Max((float)(hp/100f) * bleedPercent,1f);
            GetDamage(bleedHp, isServer);
            yield return new WaitForSeconds(1f);
        }
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

    public override void OnDestroy()
    {
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);
        if (stunCoroutine != null) StopCoroutine(stunCoroutine);
        if (bleedCoroutine != null) StopCoroutine(bleedCoroutine);
        base.OnDestroy();
    }
}

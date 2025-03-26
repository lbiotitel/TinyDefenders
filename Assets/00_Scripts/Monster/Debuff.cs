using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

//Monster Dubuff
public partial class Monster : Entity
{

    [Header("Slow Effect")]
    [SerializeField] private Color slowColor;
    private Coroutine slowCoroutine;
    private int currentSlowDebuffLevel = 0;
    private float originalSpeed;

    [Header("Stun Effect")]
    [SerializeField] private GameObject stunEffect;
    private Coroutine stunCoroutine;
    private bool isStun = false;

    [Header("Bleed Effect")]
    [SerializeField] private GameObject bleedEffect;
    private Coroutine bleedCoroutine;


    [Header("Poision Effect")]
    [SerializeField] private GameObject poisionEffect;
    private Coroutine poisionCoroutine;
    private int poisionStack;

    [Header("Ignite Effect")]
    [SerializeField] private GameObject igniteEffect;
    private Coroutine igniteCoroutine;



    [ServerRpc(RequireOwnership = false)]
    public void DebuffServerRpc(int debuffType, float[] values, bool isServer)
    {
        if (isDead) return;
  
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
                    float bleedDamagePercent = (float)values[1];
                    BleedDebuffClientRpc(bleedDamagePercent, bleedDuration, isServer);
                }
                break;
            case DebuffType.POISION:
                {
                    if (poisionCoroutine != null)
                    {
                        poisionStack++;
                    }
                    float poisionDuration = (float)values[0];
                    float poisionTickDamage = (float)values[1];
                    PoisionDebuffClientRpc(poisionDuration, poisionTickDamage,isServer);
                }
                break;

            case DebuffType.IGNITE: 
                {
                    float igniteDuration = (float)values[0];
                    float igniteTickDamage = (float)values[1];
                    IgniteDebuffClientRpc(igniteDuration,igniteTickDamage,isServer);
                }
                break;
        }
    }



    #region About Stun Effect
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
    #endregion

    #region About Bleed Effect
    [ClientRpc]
    private void BleedDebuffClientRpc(float bleedPercent, float bleedDuration, bool isServer)
    {
        CheckAndStopCoroutine(bleedCoroutine);
        Coroutine bleedDamageCoroutine = null;
        bleedCoroutine = StartCoroutine(
          DebuffCoroutine(
              bleedDuration,
              () =>
              {
                  bleedEffect.gameObject.SetActive(true);
                  bleedDamageCoroutine = StartCoroutine(BleedDamageCoroutine(isServer, bleedPercent));
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

    private IEnumerator BleedDamageCoroutine(bool isServer, float bleedPercent)
    {
        while (true)
        {
            float bleedHp = Mathf.Max((float)(hp / 100f) * bleedPercent, 1f);
            //GetDamage(bleedHp, isServer);
            yield return new WaitForSeconds(1f);
        }
    }

    #endregion

    #region About Slow Effect

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
    #endregion

    #region About Poision Effect
    [ClientRpc]
    private void PoisionDebuffClientRpc(float poisionDuration,float poisionTickDamage,bool isServer)
    {
        CheckAndStopCoroutine(poisionCoroutine);

        poisionCoroutine = StartCoroutine(
          DebuffCoroutine(
              poisionDuration,
              () =>
              {
                  poisionEffect.gameObject.SetActive(true);
                  double damage = poisionStack * poisionTickDamage;
                  //GetDamage(damage, isServer);
              },
              () =>
              {
                  poisionEffect.gameObject.SetActive(false);
                  poisionStack = 0;
              }
          )
      );

    }


    #endregion

    #region About Ignite Effect

    [ClientRpc]
    private void IgniteDebuffClientRpc(float igniteDuration, float igniteTickDamage, bool isServer)
    {
        CheckAndStopCoroutine(igniteCoroutine);

        igniteCoroutine = StartCoroutine(
          DebuffCoroutine(
              igniteDuration,
              () =>
              {
                  igniteEffect.gameObject.SetActive(true);
                  double damage = igniteTickDamage;
                  //GetDamage(damage, isServer);
              },
              () =>
              {
                  igniteEffect.gameObject.SetActive(false);
              }
          )
      );

    }

    #endregion


    private void CheckAndStopCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }


    private IEnumerator DebuffCoroutine(float debuffDuration, Action firstAction, Action secondAction)
    {
        firstAction?.Invoke();
        yield return new WaitForSeconds(debuffDuration);
        secondAction?.Invoke();
    }


    public override void OnDestroy()
    {
        StopAllDebuff();
        base.OnDestroy();
    }

    private void StopAllDebuff()
    {
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);
        if (stunCoroutine != null) 
        {
            StopCoroutine(stunCoroutine);
            stunEffect.gameObject.SetActive(false);
            isStun = false;
        }
        if (igniteCoroutine != null)
        {
            StopCoroutine(igniteCoroutine);
            igniteEffect.gameObject.SetActive(false);
        }
        if (bleedCoroutine != null) StopCoroutine(bleedCoroutine);
        if(poisionCoroutine != null)StopCoroutine(poisionCoroutine);
    }
}

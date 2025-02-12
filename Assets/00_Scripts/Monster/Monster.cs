using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Monster : Entity
{
    [SerializeField] private float speed;
    [SerializeField] private int maxHp = 0; 
    private int hp = 0;
    [SerializeField] private DamageText damageText;
    [SerializeField] private Image hpImage;
    [SerializeField] private Image hpImageEffect;
    [SerializeField] private float hpAnimeSpeed = 2f;

    int currentTarget = 1;
    List<Transform> targetLists;
    private bool isDead = false;

    public override void Start()
    {
        hp = maxHp;
        base.Start();
    }

    public void SetMoveTarget(List<Transform> targetLists)
    {
        this.targetLists = targetLists;
    }

    public void GetDamage(int damage)
    {
        if (isDead) return;

        hp -= damage;
        hpImage.fillAmount = ((float)hp / (float)maxHp);
        Instantiate(damageText, transform.position, Quaternion.identity).InitDamageText(damage);

        if (hp <= 0)
        {
            isDead = true;
            GameManager.instance.RemoveMonster(this);
            GameManager.instance.GetGold(1);
            gameObject.layer = LayerMask.NameToLayer("Default");
            StartCoroutine(DeadCoroutine());
            AnimatorChange("DEAD",true);
        }
    }

    private void Update()
    {
        MoveLogic();
        HpSlideAnimation();
    }

    private void HpSlideAnimation()
    {
        hpImageEffect.fillAmount = Mathf.Lerp(hpImageEffect.fillAmount, hpImage.fillAmount,Time.deltaTime * hpAnimeSpeed);
    }

    IEnumerator DeadCoroutine()
    {
        float alpha = 1.0f;

        while (spriteRenderer.color.a > 0.0f)
        {
            alpha -= Time.deltaTime;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b,
                alpha);
            yield return null;  
        }

        if (IsServer)
        { 
            Destroy(this);
        }
        else{
            DestroyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyServerRpc()
    {
        Destroy(this);
    }

    public bool IsDead() { return isDead; }

    private void MoveLogic()
    {
        if (isDead) return;
        if (Vector2.Distance(transform.position, targetLists[currentTarget].position) <= 0.0f)
        {
            spriteRenderer.flipX = currentTarget >= 2 ? true : false;
            currentTarget = (currentTarget + 1) % targetLists.Count;
        }

        transform.position = Vector2.MoveTowards(transform.position, targetLists[currentTarget].position, Time.deltaTime * speed);
    }
}

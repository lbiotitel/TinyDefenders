using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class Monster : Entity
{
    [SerializeField] private float speed = 0f;
    [SerializeField] private double maxHp = 0; 
    private double hp = 0;
    [SerializeField] private DamageText damageText;
    [SerializeField] private Image hpImage;
    [SerializeField] private Image hpImageEffect;
    [SerializeField] private float hpAnimeSpeed = 2f;

    int currentMoveTarget = 1;
    List<Transform> targetLists;
    private bool isDead = false;
    private MonsterType monsterType = MonsterType.NORMAL;

    public override void Awake()
    {
        base.Awake();
    }

    public void InitData(MonsterScriptable data)
    {
        hp = maxHp = data.monsterHp;
        speed = originalSpeed = data.monsterSpeed;
        animator.runtimeAnimatorController = data.animatorController;
        monsterType = data.monsterType;
    }

    public void SetMoveTarget(List<Transform> targetLists)
    {
        this.targetLists = targetLists;
    }

    public void GetDamage(double damage)
    {
        if (!IsServer) return;
        if (isDead) return;
        
        MonsterGetDamage(damage);
        UpdateClientRpc(this.hp,damage);
    }

    private void MonsterGetDamage(double damage)
    {
        hp -= (int)damage;
        hpImage.fillAmount = ((float)hp / (float)maxHp);
        Instantiate(damageText, transform.position, Quaternion.identity).InitDamageText(damage);
        if (hp <= 0)
        {
            isDead = true;
            gameObject.layer = LayerMask.NameToLayer("Default");
            GameManager.instance.GetGold(1);
            StartCoroutine(DeadCoroutine());
            AnimatorChange("DEAD", true);
        }
    }

    [ClientRpc]
    private void UpdateClientRpc(double remainHp, double damage)
    {
        hp = remainHp;
        hpImage.fillAmount = ((float)hp / (float)maxHp);
        Instantiate(damageText, transform.position, Quaternion.identity).InitDamageText(damage);
        if (hp <= 0)
        {
            isDead = true;
            gameObject.layer = LayerMask.NameToLayer("Default");
            StartCoroutine(DeadCoroutine());
            AnimatorChange("DEAD", true);
        }
    }

    private void Update()
    {
        if (isDead) return;    
        if (isStun) return;
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
            if (monsterType == MonsterType.BOSS)
                GameManager.instance.BossMonsterDead();
            this.gameObject.SetActive(false);
            GameManager.instance.RemoveMonster(this);
            NetworkBehaviour.Destroy(this);
        }
        else { 
            this.gameObject.SetActive(false);
        }
    }

    public bool IsDead() { return isDead; }

    private void MoveLogic()
    {
        if (isDead) return;
        if (Vector2.Distance(transform.position, targetLists[currentMoveTarget].position) <= 0.0f)
        {
            spriteRenderer.flipX = currentMoveTarget >= 2 ? true : false;
            currentMoveTarget = (currentMoveTarget + 1) % targetLists.Count;
        }

        transform.position = Vector2.MoveTowards(transform.position, targetLists[currentMoveTarget].position, Time.deltaTime * speed);
    }
}

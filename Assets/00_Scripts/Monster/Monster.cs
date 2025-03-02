using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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
    [SerializeField] private float standAngle;
    [SerializeField] private GameObject deadEffect;
    
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
        transform.Rotate(standAngle, 0, 0);
    }

    public void SetMoveTarget(List<Transform> targetLists)
    {
        this.targetLists = targetLists;
    }

    public void GetDamage(double damage, bool isServer)
    {
        if (!IsServer) return;
        if (isDead) return;

        MonsterGetDamage(damage, isServer);
        UpdateClientRpc(this.hp, damage);
    }

    private void MonsterGetDamage(double damage, bool isServer)
    {
        if (isServer)
        {
            GameManager.instance.playerDps += damage;
        }
        else
        {
            GameManager.instance.friendDps += damage;
        }

        hp -= (int)damage;
        hpImage.fillAmount = ((float)hp / (float)maxHp);
        Dead();
        GetReward();
    }

    [ClientRpc]
    private void UpdateClientRpc(double remainHp, double damage)
    {
        hp = remainHp;
        hpImage.fillAmount = ((float)hp / (float)maxHp);
        Instantiate(damageText, transform.position, Quaternion.identity).InitDamageText(damage);
        Dead();
    }

    private void Dead()
    {
        if (hp <= 0)
        {
            isDead = true;
            gameObject.layer = LayerMask.NameToLayer("Default");
            StartCoroutine(DeadCoroutine());
            AnimatorChange("DEAD", true);
            Instantiate(deadEffect,transform.position,Quaternion.identity);
        }
    }

    private void GetReward()
    {
        if (monsterType == MonsterType.NORMAL)
        {
            GameManager.instance.GetGold(1);
        }
        else if (monsterType == MonsterType.GOLD)
        {
            int rewardGold = GameManager.instance.goldGoblinLevel * GameManager.instance.goldGoblinRewardGold;
            GameManager.instance.GetGold(rewardGold);
        }
    }

    private void Update()
    {
        if (isDead) return;
        HpSlideAnimation();
        if (isStun) return;
        MoveLogic();
    }

    private void HpSlideAnimation()
    {
        hpImageEffect.fillAmount = Mathf.Lerp(hpImageEffect.fillAmount, hpImage.fillAmount, Time.deltaTime * hpAnimeSpeed);
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
        else
        {
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

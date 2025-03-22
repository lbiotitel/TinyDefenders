using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public partial class Monster : Entity
{
    [Header("Monster Basic Property")]
    [SerializeField] private DamageText damageText;
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private GameObject deadParticlePrefab;
    [SerializeField] private Image hpImage;
    [SerializeField] private Image hpImageEffect;
    [SerializeField] private float hpAnimeSpeed = 2f;

    private float speed = 0f;
    private double maxHp = 0;
    private double hp = 0;


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
        ResetObject();
        hp = maxHp = data.monsterHp;
        speed = originalSpeed = data.monsterSpeed;
        animator.runtimeAnimatorController = data.animatorController;
        monsterType = data.monsterType;
    }

    private void ResetObject()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        isDead = false;
        gameObject.layer = LayerMask.NameToLayer(LayerName.Monster);

        Color originColor = spriteRenderer.color;
        originColor.a = 1f;
        spriteRenderer.color = originColor;
        spriteRenderer.flipX = false;

        animator.Rebind();
        spriteRenderer.flipX = true;
        currentMoveTarget = 1;
        hpImage.fillAmount = 1f;
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
        GetReward();
        UpdateClientRpc(hp,damage);
    }

    [ClientRpc]
    private void UpdateClientRpc(double remainHp, double damage)
    {
        hp = remainHp;
        hpImage.fillAmount = ((float)hp / (float)maxHp);
        DamageText damageText = PoolingManager.instance.GetObject<DamageText>(damageTextPrefab, transform.position);
        damageText.InitDamageText(damage, true);
        Dead();
    }

    private void Dead()
    {
        if (hp <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                StopAllDebuff();
                gameObject.layer = LayerMask.NameToLayer(LayerName.Defulat);
                StartCoroutine(DeadCoroutine());
                AnimatorChange("DEAD", true);
                targetLists = null;
            }
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
        SyncMonsterPosition();
    }

    private void SyncMonsterPosition()
    {
        if (NetUtils.IsServer())
        {
            MoveLogic();
        }
    }

    private void HpSlideAnimation()
    {
        hpImageEffect.fillAmount = Mathf.Lerp(hpImageEffect.fillAmount, hpImage.fillAmount, Time.deltaTime * hpAnimeSpeed);
    }

    IEnumerator DeadCoroutine()
    {
        GameObject deadParticle = PoolingManager.instance.GetObject(deadParticlePrefab, transform.position);
        float alpha = 1.0f;

        while (spriteRenderer.color.a > 0.0f)
        {
            alpha -= Time.deltaTime;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b,
                alpha);
            yield return null;
        }

        PoolingManager.instance.ReturnObject(deadParticle);

        if (IsServer)
        {
            if (monsterType == MonsterType.BOSS)
                GameManager.instance.BossMonsterDead();
            GetComponent<NetworkObject>().Despawn();
            GameManager.instance.RemoveMonster(this);
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

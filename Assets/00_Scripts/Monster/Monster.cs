using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SocialPlatforms;
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
    [SerializeField] private string monsterDeathSFX;

    private float speed = 0f;
    public double maxHp { get; private set; }
    public double hp { get; private set; }


    int currentMoveTarget = 1;
    List<Transform> movePointList;
    private double defense = 0;
    private bool isDead = false;
    private MonsterType monsterType = MonsterType.NORMAL;

    public override void Awake()
    {
        base.Awake();
    }

    #region Monster Init Logic

    public void InitData(MonsterScriptable data, List<Transform> targetLists)
    {
        StartCoroutine(ResetObject());
        hp = maxHp = data.monsterHp;
        defense = data.defense;
        speed = originalSpeed = data.monsterSpeed;
        animator.runtimeAnimatorController = data.animatorController;
        monsterType = data.monsterType;
        this.movePointList = targetLists;
    }

    private void SyncMonsterPosition()
    {
        if (NetUtils.IsHost())
        {
            MoveLogic();
        }
    }

    private IEnumerator ResetObject()
    {
        yield return new WaitForSeconds(0.2f);

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

    #endregion

    #region Monster Damage Logic

    public void GetDamage(double damage, PlayerNetworkType playerNetworkType, ulong heroId, bool eventCall = true)
    {
        if (!IsServer) return;
        if (isDead) return;

        MonsterGetDamage(damage, playerNetworkType, heroId, eventCall);
    }

    public void GetDamage(double damage, PlayerNetworkType playerNetworkType)
    {
        if (!IsServer) return;
        if (isDead) return;

        MonsterGetDamage(damage,playerNetworkType);
    }

    private void MonsterGetDamage(double damage, PlayerNetworkType playerNetworkType)
    {
        damage -= defense;
        hp -= ((int)damage);
        hpImage.fillAmount = ((float)hp / (float)maxHp);
        UpdateClientRpc(hp, damage, false);
    }

    private void MonsterGetDamage(double damage, PlayerNetworkType playerNetworkType, ulong heroId, bool eventCall = true)
    {
        if (isDead) return;
        if (NetUtils.TryGetSpawnedObject(heroId, out NetworkObject heroObject))
        {
            Hero hero = heroObject.GetComponent<Hero>();

            bool isCri = IsCri(hero);
            if (isCri) damage *= 1.5f;

            damage -= defense;
            hp -= ((int)damage);


            StackDamage(damage, playerNetworkType, hero);

            hpImage.fillAmount = ((float)hp / (float)maxHp);
            UpdateClientRpc(hp, damage, isCri);

            if(eventCall)EventManager.instance.onHeroAttackToMonsterAction?.Invoke(hero, this);
        }
    }

    private bool IsCri(Hero hero)
    {
        float cri = Random.Range(0,100f);
        if (cri < hero.GetHeroCriticalRate()) return true;
        return false;
    }

    public void ExcutionMonster(PlayerNetworkType playerNetworkType, Hero hero)
    {
        if (isDead) return;

        double damage = hp < 0 ? 0 : hp;
        hp = 0;

        if (damage == 0) return;

        StackDamage(damage,playerNetworkType,hero);
        StackPlayerDamage(damage, playerNetworkType);
        UpdateClientRpc(hp, damage, false);
    }

    private void StackDamage(double damage, PlayerNetworkType playerNetworkType, Hero hero)
    {
        if (hp <= 0) damage += hp;

        StackPlayerDamage(damage, playerNetworkType);
        hero.IncreaseDamageStack(damage);
    }

    private static void StackPlayerDamage(double damage, PlayerNetworkType playerNetworkType)
    {
        if (playerNetworkType == PlayerNetworkType.HOST)
        {
            GameManager.instance.playerDamage += damage;
        }
        else
        {
            GameManager.instance.friendDamage += damage;
        }
    }

    [ClientRpc]
    private void UpdateClientRpc(double remainHp, double damage,bool isCri)
    {
        hp = remainHp;
        hpImage.fillAmount = ((float)hp / (float)maxHp);
        DamageText damageText = PoolingManager.instance.GetObject<DamageText>(damageTextPrefab, transform.position);
        damageText.InitDamageText(damage, isCri);
        Dead();
    }

    #endregion


    private void GetReward()
    {
        if (monsterType == MonsterType.NORMAL)
        {
            GameManager.instance.GetGold(1,PlayerNetworkType.ALL);
        }
        else if (monsterType == MonsterType.GOLD)
        {
            int rewardGold = GameManager.instance.goldGoblinLevel * GameManager.instance.goldGoblinRewardGold;
            GameManager.instance.GetGold(rewardGold, PlayerNetworkType.ALL);
        }
    }

    private void Update()
    {
        if (isDead) return;
        HpSlideAnimation();
        if (isStun) return;
        SyncMonsterPosition();
    }



    private void HpSlideAnimation()
    {
        hpImageEffect.fillAmount = Mathf.Lerp(hpImageEffect.fillAmount, hpImage.fillAmount, Time.deltaTime * hpAnimeSpeed);
    }


    #region Monster Dead Logic
    private void Dead()
    {
        if (hp <= 0)
        {
            if (!isDead)
            {
                if (IsServer) GetReward();

                SoundManager.instance.PlaySfxSound(monsterDeathSFX,SFXTYPE.EXTRA);
                isDead = true;
                StopAllDebuff();
                gameObject.layer = LayerMask.NameToLayer(LayerName.Defulat);
                StartCoroutine(DeadCoroutine());
                AnimatorChange("DEAD", true);
                movePointList = null;
            }
        }
    }

    IEnumerator DeadCoroutine()
    {
        PoolingManager.instance.GetObject(deadParticlePrefab, transform.position);
        float alpha = 1.0f;

        while (spriteRenderer.color.a > 0.0f)
        {
            alpha -= Time.deltaTime;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b,
                alpha);
            yield return null;
        }

        GetComponent<NetworkTransform>().Interpolate = false;

        if (IsServer)
        {
            if (monsterType == MonsterType.BOSS)
                GameManager.instance.BossMonsterDead();
            GetComponent<NetworkObject>().Despawn();
            GameManager.instance.RemoveMonster(this);
        }
    }

    public bool IsDead() { return isDead; }

    #endregion

    private void MoveLogic()
    {
        if (isDead) return;
        if (movePointList == null) return;
        if (Vector2.Distance(transform.position, movePointList[currentMoveTarget].position) <= 0.0f)
        {
            spriteRenderer.flipX = currentMoveTarget >= 2 ? true : false;
            currentMoveTarget = (currentMoveTarget + 1) % movePointList.Count;
        }

        transform.position = Vector2.MoveTowards(transform.position, movePointList[currentMoveTarget].position, Time.deltaTime * speed);
    }


}

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.UI;

public class DamageEvent { 
    public double originDamage;
    public double finalDamage;
    public Monster monster;
    public Hero hero;

    public DamageEvent(double originDamage,double finalDamage,Monster monster,Hero hero)
    { 
        this.finalDamage = finalDamage;
        this.originDamage = originDamage;
        this.monster = monster;
        this.hero = hero;
    }
}

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
    [SerializeField] private AnimationCurve monsterHpCurve;


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

    public void InitData(MonsterScriptable data, List<Transform> movePointList)
    {
        ResetObject();
        CalcMonsterHp(data);
        defense = data.defense;
        speed = originalSpeed = data.monsterSpeed;
        animator.runtimeAnimatorController = data.animatorController;
        monsterType = data.monsterType;
        this.movePointList = movePointList;
    }

    private void CalcMonsterHp(MonsterScriptable data)
    {
        int currentWave = GameManager.instance.GetCurrentWave();
        float result = monsterHpCurve.Evaluate(currentWave/50f);
        maxHp = data.monsterHp;
        maxHp = maxHp + (maxHp * (result * 1.3f));
        hp = maxHp;
    }

    private void SyncMonsterPosition()
    {
        if (NetUtils.IsHost())
        {
            MoveLogic();
        }
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

        MonsterGetDamage(damage, playerNetworkType);
    }

    private void MonsterGetDamage(double damage, PlayerNetworkType playerNetworkType)
    {
        hp -= CalcDamage(damage);
        UpdateClientRpc(hp, damage, false);
    }

    private void MonsterGetDamage(double damage, PlayerNetworkType playerNetworkType, ulong heroId, bool eventCall = true)
    {
        if (isDead) return;
        if (NetUtils.TryGetSpawnedObject(heroId, out NetworkObject heroObject))
        {
            Hero hero = heroObject.GetComponent<Hero>();

            bool isCri = false;
            double finalDamage = CalcDamage(damage, hero, ref isCri);

            DamageEvent damageEvent = new DamageEvent(damage, finalDamage, this, hero);
            if (eventCall) EventManager.instance.onMonsterBeforeTakeDamageAction?.Invoke(damageEvent);
            finalDamage = Mathf.RoundToInt((float)damageEvent.finalDamage);
            
            hp -= finalDamage;

            StackDamage(finalDamage, playerNetworkType, hero);
            UpdateClientRpc(hp, finalDamage, isCri);

            if (eventCall) EventManager.instance.onHeroAttackToMonsterAction?.Invoke(hero, this);
        }
    }

    private double CalcDamage(double damage, Hero hero, ref bool isCri)
    {
        isCri = false;
        if (hero != null)
        {
            isCri = IsCri(hero);
            if (isCri) damage *= hero.GetHeroData().GetHeroCriDamage();
        }

        damage -= defense;
        return Mathf.RoundToInt((float)damage); ;

    }

    private double CalcDamage(double damage)
    {
        damage -= defense;
        return Mathf.RoundToInt((float)damage); 
    }


    private bool IsCri(Hero hero)
    {
        float cri = Random.Range(0, 100f);
        if (cri < hero.GetHeroData().GetHeroCriticalRate()) return true;
        return false;
    }

    public void ExcutionMonster(PlayerNetworkType playerNetworkType, Hero hero)
    {
        if (isDead) return;

        double damage = hp < 0 ? 0 : hp;
        hp = 0;

        if (damage == 0) return;

        StackDamage(damage, playerNetworkType, hero);
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
    private void UpdateClientRpc(double remainHp, double damage, bool isCri)
    {
        hp = remainHp;
        hpImage.fillAmount = ((float)hp / (float)maxHp);
        DamageText damageText = PoolingManager.instance.GetObject<DamageText>(damageTextPrefab, transform.position);
        damageText.InitDamageText(damage, isCri);
        CheckDead();
    }

    #endregion


    private void Update()
    {
        HpSlideAnimation();
        if (isDead) return;
        if (isStun) return;
        SyncMonsterPosition();
    }



    private void HpSlideAnimation()
    {
        hpImageEffect.fillAmount = Mathf.Lerp(hpImageEffect.fillAmount, hpImage.fillAmount, Time.deltaTime * hpAnimeSpeed);
    }


    #region Monster Dead Logic
    private void CheckDead()
    {
        if (hp <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                SoundManager.instance.PlaySfxSound(monsterDeathSFX, SFXTYPE.EXTRA);
                StopAllDebuff();
                gameObject.layer = LayerMask.NameToLayer(LayerName.Defulat);
                StartCoroutine(DeadCoroutine());
                AnimatorChange("DEAD", true);
                movePointList = null;

                EventManager.instance.onMonsterDeadAction?.Invoke(this);
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
            SpawnManager.instance.RemoveMonster(this);
        }
    }

    public bool IsDead() { return isDead; }

    [ClientRpc]
    public void DeadClientRpc() {
        hp = 0;
        CheckDead();
    }

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

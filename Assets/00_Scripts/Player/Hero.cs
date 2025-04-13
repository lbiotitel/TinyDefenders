using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using WebSocketSharp;

public class Hero : Entity
{
    [SerializeField] private GameObject spawnParticle;
    [SerializeField] private ParticleSystem rarityParticle;
    [SerializeField] private CircleCollider2D heroRangeCollider;
    [SerializeField] private ParticleSystem upgradeParticle;

    private HeroScriptable heroData;
    private HeroHolder heroHolder;

    private bool isMove = false;
    private bool attackDelay = false;

    private List<Monster> monsterInRangeList = new List<Monster>();
    private double damageStack = 0;

    [field: SerializeField] public LayerMask monsterLayer { get; private set; }
    public string heroName { get; set; }
    [HideInInspector] public HeroRarity heroRarity;
    [HideInInspector] public NetworkObject target { get; set; } = null;
    public PlayerNetworkType heroOwnerNetworkType { get; private set; }


    public override void Awake()
    {
        base.Awake();
    }

    public void InitHeroData(HeroScriptable data, HeroHolder heroHolder)
    {
        if (GameManager.instance.isGameOver) return;

        heroData = Instantiate(data);

        heroName = heroData.heroName;
        heroRarity = heroData.rarity;
        this.heroHolder = heroHolder;
        animator.runtimeAnimatorController = data.runtimeAnimatorController;

        InitHeroRangeCollider();
        InitRarityCircleColor();

        Instantiate(spawnParticle, heroHolder.transform.position, Quaternion.identity);

        heroOwnerNetworkType = 
            heroHolder.holderId.Contains("CLIENT") ? PlayerNetworkType.CLIENT : PlayerNetworkType.HOST;

        heroData.InitHeroNetworkType(heroOwnerNetworkType);

        SpawnManager.instance.AddHero(this);
        EventManager.instance.onUpgradeAction += PlayUpgradeParticle;
    }


    private void InitHeroRangeCollider()
    {
        float rad = heroRangeCollider.radius;
        heroRangeCollider.radius = heroData.GetHeroAtkRange();
    }

    private void InitRarityCircleColor()
    {
        ParticleSystem.MainModule main = rarityParticle.main;
        main.startColor = Utils.GetRarityColor(heroRarity);
    }

    private void Update()
    {
        if (isMove) return;
        CheckForEnemies();
    }

    public void SwapPosition(HeroHolder target, Vector2 targetPos)
    {
        isMove = true;
        AnimatorChange("MOVE", false);

        if (IsServer)
            transform.parent = target.transform;

        heroHolder = target;

        int sign = (int)Mathf.Sign(targetPos.x - transform.position.x);

        StartCoroutine(SwapCoroutine(targetPos));
    }

    private IEnumerator SwapCoroutine(Vector2 targetPos)
    {
        float current = 0.0f;
        float percent = 0.0f;
        Vector2 startPos = transform.position;
        Vector2 endPos = targetPos;

        while (percent < 1.0f)
        {
            current += Time.deltaTime;
            percent = current / 0.5f;

            Vector2 lerpPos = Vector2.Lerp(startPos, endPos, percent);
            transform.position = lerpPos;
            yield return null;
        }

        isMove = false;
        AnimatorChange("IDLE", false);
    }

    private void CheckForEnemies()
    {
        if (heroData.projectile == null) return;
        if (IsServer)
        {
            if (attackDelay) return;

            target = null;

            if (monsterInRangeList.Count > 0)
            {
                target = FindCloseMonster()?.GetComponent<NetworkObject>();

                if (target != null)
                {
                    attackDelay = true;
                    StartCoroutine(AttackDelayCoroutine());
                    LoadBulletClientRpc(target.NetworkObjectId);
                }

            }
            else
                target = null;
        }
    }

    private GameObject FindCloseMonster()
    {
        GameObject go = null;
        float distance = float.MaxValue;

        foreach (var monster in monsterInRangeList)
        {
            if (monster.IsDead()) continue;
            if (monster != null)
            {
                float currentDistance = Vector2.Distance(transform.position, monster.transform.position);
                if (currentDistance < distance)
                {
                    go = monster.gameObject;
                    distance = currentDistance;
                }
            }
        }

        return go;
    }


    private IEnumerator AttackDelayCoroutine()
    {
        yield return new WaitForSeconds(1f / heroData.GetHeroAtkSpeed());
        attackDelay = false;
    }


    [ClientRpc]
    private void LoadBulletClientRpc(ulong networkObjectId)
    {
        if (NetUtils.TryGetSpawnedObject(networkObjectId, out NetworkObject targetObject))
        {
            AnimatorChange("ATTACK", true);
            BaseProjectile go =
                PoolingManager.instance.GetObject<BaseProjectile>(heroData.projectile.gameObject, transform.position);
            go.Init(targetObject.transform,this, heroData.GetHeroAtk());
            LooKTarget(targetObject.gameObject);
        }
    }

    private void LooKTarget(GameObject target)
    {
        if (target.transform.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {

            spriteRenderer.flipX = true;
        }
    }

    public void PlayUpgradeParticle(HeroTribe heroTribe,PlayerNetworkType playerNetworkType)
    {
        if(heroTribe == heroData.heroTribe && playerNetworkType == heroOwnerNetworkType)
            upgradeParticle.Play();
    }


    #region Collision Check

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            monsterInRangeList.Add(collision.GetComponent<Monster>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            monsterInRangeList.Remove(collision.GetComponent<Monster>());
        }
    }

    #endregion


  
    public HeroScriptable GetHeroData() { return heroData; }
    public HeroHolder GetHeroHolder() { return heroHolder; }
    public void IncreaseDamageStack(double damage) { damageStack += damage; }
    public double GetDamageStack() { return damageStack; }
    public void SetMonsterKillCount(double damageStack) { this.damageStack = damageStack; }
    public List<Monster> GetMonsterList() { return monsterInRangeList; }


    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (GameManager.instance.isGameOver) return;
        EventManager.instance.onUpgradeAction -= PlayUpgradeParticle;
        SpawnManager.instance.RemoveHero(this);
        Destroy(heroData);
    }
}

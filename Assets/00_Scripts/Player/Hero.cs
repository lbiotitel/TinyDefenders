using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Hero : Entity
{
    [SerializeField] private GameObject spawnParticle;
    [SerializeField] private ParticleSystem rarityParticle;
    [SerializeField] private PolygonCollider2D heroRangeCollider;

    private HeroScriptable heroData;
    private HeroHolder heroHolder;

    private bool isMove = false;
    private bool attackDelay = false;
    bool isServerHero;

    private List<Monster> monsterInRangeList = new List<Monster>();
    private int monsterKillCount = 0;

    [field: SerializeField] public LayerMask monsterLayer { get; private set; }
    public string heroName { get; set; }
    [HideInInspector] public HeroRarity heroRarity;
    [HideInInspector] public NetworkObject target { get; set; } = null;


    public override void Awake()
    {
        base.Awake();
    }

    public void InitHeroData(HeroScriptable data, HeroHolder heroHolder, string rarity)
    {
        heroData = data;

        heroName = heroData.heroName;
        heroRarity = (HeroRarity)Enum.Parse(typeof(HeroRarity), rarity);
        this.heroHolder = heroHolder;

        InitHeroRangeCollider();

        GetInitEntity(data.heroName, rarity);

        InitRarityCircleColor();

        InitHeroSkill();

        GameManager.instance.AddHero(this);

        Instantiate(spawnParticle, heroHolder.transform.position, Quaternion.identity);

        isServerHero = heroHolder.holderId.Contains("CLIENT") ? false : true;
    }

    private void InitHeroSkill()
    {
        Type classType = Type.GetType(heroName + "Skill");
        if (classType != null)
            gameObject.AddComponent(classType);
    }


    private void InitHeroRangeCollider()
    {
        Vector2[] points = heroRangeCollider.points;

        for (int i = 0; i < points.Length; i++)
        {
            points[i] *= heroData.atkRange.baseValue;
        }

        heroRangeCollider.points = points;
    }

    private void InitRarityCircleColor()
    {
        ParticleSystem.MainModule main = rarityParticle.main;
        main.startColor = Utils.GetRarityColor(heroRarity);
    }

    private int GetUpgradeIndex()
    {
        switch (heroRarity)
        {
            case HeroRarity.Normal:
            case HeroRarity.Uncommon:
                return 0;

            case HeroRarity.Rare:
                return 1;

            case HeroRarity.Legend:
                return 2;
        }

        return -1;
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
        yield return new WaitForSeconds(1f / heroData.atkSpeed.GetValue());
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
            go.Init(targetObject.transform, this);
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

    public override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.instance.RemoveHero(this);
    }

    public double GetHeroAtk()
    {
        double atk = heroData.atk.GetValue();
        atk += atk * (GameManager.instance.GetHeroUpgradeLevel(heroData.heroTribe, isServerHero) * 0.05f);
        return atk;
    }

    public double GetBonusHeroAtk()
    {
        return GetHeroAtk() - heroData.atk.baseValue;
    }

    public float GetHeroAtkSpeed()
    {
        return heroData.atkSpeed.GetValue();
    }

    public float GetBounusHeroAtkSpeed()
    {
        return heroData.atk.GetModifireValue();
    }

    public float GetHeroAtkRange()
    {
        return heroData.atkRange.GetValue();
    }

    public float GetHeroBonusAtkRange()
    {
        return heroData.atkRange.GetModifireValue();
    }

    public HeroScriptable GetHeroData() { return heroData; }
    public HeroHolder GetHeroHolder() { return heroHolder; }
    public void IncreaseMonsterKillCount() { monsterKillCount++; }
    public int GetMonsterKillCount() { return monsterKillCount; }
    public void SetMonsterKillCount(int killCount) { this.monsterKillCount = killCount; }
    public List<Monster> GetMonsterList() { return monsterInRangeList; }
}

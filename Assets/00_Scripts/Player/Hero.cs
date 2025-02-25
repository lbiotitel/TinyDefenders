using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Hero : Entity
{
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private GameObject spawnParticle;
    HeroScriptable heroData;
    private HeroHolder heroHolder;
    private bool isMove = false;
    private double baseAtk;
    [SerializeField] private float attackDelay;
    public double atk
    {
        get
        {
            float uggradeBonus =
                GameManager.instance.upgarde[GetUpgradeIndex()] != 0 ?
                GameManager.instance.upgarde[GetUpgradeIndex()] * 0.1f : 0;
            return baseAtk * (1 + uggradeBonus);
        }
        set
        {
        }
    }
    [field: SerializeField] public float attackRange { get; set; } = 1f;
    [field: SerializeField] public float attackSpeed;
    [field: SerializeField] public LayerMask monsterLayer { get; private set; }
    public string heroName { get; set; }
    public Rarity heroRarity;
    public NetworkObject target { get; set; } = null;

    [SerializeField] private Color[] circleColors;
    [SerializeField] private SpriteRenderer circleSpriteRenderer;

    public override void Awake()
    {
        base.Awake();
    }

    public void InitHeroData(HeroData data, HeroHolder heroHolder, string rarity)
    {
        heroData = Resources.Load<HeroScriptable>("HeroDatas/" + rarity + "/" + data.heroName);

        heroName = data.heroName;
        heroRarity = (Rarity)Enum.Parse(typeof(Rarity), rarity);
        baseAtk = data.heroAtk;
        attackRange = data.heroAtkRange;
        attackSpeed = data.heroAtkSpeed;
        this.heroHolder = heroHolder;

        circleSpriteRenderer.color = circleColors[(int)heroRarity];

        attackDelay = attackSpeed;

        Type classType = Type.GetType(heroName + "Skill");
        if (classType != null)
            gameObject.AddComponent(classType);


        GetInitEntity(data.heroName, rarity);

        InitEquipment();

        GameManager.instance.AddHero(this);

        Instantiate(spawnParticle, heroHolder.transform.position, Quaternion.identity);
    }

    private int GetUpgradeIndex()
    {
        switch (heroRarity)
        {
            case Rarity.Normal:
            case Rarity.Uncommon:
                return 0;

            case Rarity.Unique:
                return 1;

            case Rarity.Legend:
                return 2;
        }

        return -1;
    }

    private void InitEquipment()
    {
        if (heroData.leftEquipmentPrefab != null)
        {
            GameObject leftEquipment = Instantiate(heroData.leftEquipmentPrefab);
            leftEquipment.transform.SetParent(leftHand);
            leftEquipment.transform.localPosition = Vector3.zero;
        }

        if (heroData.rightEquipmentPrefab != null)
        {
            GameObject rightEquipment = Instantiate(heroData.rightEquipmentPrefab);
            rightEquipment.transform.SetParent(rightHand);
            rightEquipment.transform.localPosition = Vector3.zero;
        }
    }


    private void Update()
    {
        if (isMove) return;
        CheckForEnemies();
    }

    public void SwapPosition(HeroHolder target, Vector2[] targetPos, int index)
    {
        isMove = true;
        AnimatorChange("MOVE", false);

        if (IsServer)
            transform.parent = target.transform;

        heroHolder = target;

        int sign = (int)Mathf.Sign(targetPos[index].x - transform.position.x);

        switch (sign)
        {
            case -1:
                transform.Rotate(0, 180, 0);
                break;
            case 1:
                transform.Rotate(0, -180, 0);
                break;
        }

        StartCoroutine(SwapCoroutine(targetPos[index]));
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
        transform.Rotate(0, 180, 0);
        AnimatorChange("IDLE", false);
    }

    private void CheckForEnemies()
    {
        if (IsServer)
        {
            if (attackDelay > 0)
            {
                attackDelay -= Time.deltaTime;
                return;
            }

            Collider2D[] enemiesInRange =
                Physics2D.OverlapCircleAll(heroHolder.transform.position, attackRange, monsterLayer);

            if (enemiesInRange.Length > 0)
            {
                for (int i = 0; i < enemiesInRange.Length; i++)
                {
                    if (enemiesInRange[i].GetComponent<Monster>().IsDead())
                    {
                        continue;
                    }
                    else
                    {
                        target = enemiesInRange[i].transform.GetComponent<NetworkObject>();
                        break;
                    }
                }

                if (target != null)
                {
                    attackDelay = attackSpeed;
                    if (heroData.projectile == null) return;
                    LoadBulletClientRpc(target.NetworkObjectId);
                }
            }
            else
                target = null;
        }
    }



    [ClientRpc]
    private void LoadBulletClientRpc(ulong networkObjectId)
    {
        if (NetUtils.TryGetSpawnedObject(networkObjectId, out NetworkObject targetObject))
        {
            AnimatorChange("ATTACK", true);
            Projectile go = Instantiate(heroData.projectile, transform.position, Quaternion.identity);
            go.Init(targetObject.transform, this);
            target = targetObject;
        }
    }

    public void AttackMonster()
    {
        if (target != null)
        {
            AttackMonsterServerRpc(target.NetworkObjectId,heroHolder.holderId.Contains("CLIENT") ? false:true);
            DebuffEffect();
        }
    }

    private void DebuffEffect()
    {
        for (int i = 0; i < heroData.debuffEffects.Count; i++)
        {
            DebuffEffect debuffEffect = heroData.debuffEffects[i];
            DebuffType debuffType = debuffEffect.debuffType;
            float chance = debuffEffect.values[0];

            List<float> values = new List<float>(debuffEffect.values);
            values.RemoveAt(0);

            float[] sendValues = values.ToArray();

            if (UnityEngine.Random.value <= chance)
            {
                target.GetComponent<Monster>().DebuffServerRpc(
                    (int)debuffType, sendValues
                    );
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AttackMonsterServerRpc(ulong monsterId,bool isServer)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(monsterId, out NetworkObject networkObject))
        {
            Monster monster = networkObject.GetComponent<Monster>();
            if (monster != null)
            {
                monster.GetDamage(atk,isServer);
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.instance.RemoveHero(this);
    }
    public HeroScriptable GetHeroData() { return heroData; }
}

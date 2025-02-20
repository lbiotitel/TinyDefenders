using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Hero : Entity
{
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private GameObject spawnParticle;
    HeroScriptable heroData;
    private HeroHolder heroHolder;
    private bool isMove = false;

    private double baseAtk;

    public float stunChance = 0.5f;
    public float slowChance = 0.5f;
    public float slowAmount = 0.5f;
    public float slowDuration = 2f;

    public double atk 
    {
        get {
            float uggradeBonus = 
                GameManager.instance.upgarde[GetUpgradeIndex()] != 0 ?
                GameManager.instance.upgarde[GetUpgradeIndex()] * 0.1f : 0;
            return baseAtk * (1 + uggradeBonus);
        }
        set { 
        }
    }
    [field:SerializeField] public float attackRange { get; set; } = 1f;
    [field: SerializeField] public float attackSpeed { get; private set; } = 1.0f;
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

    public void InitHeroData(HeroData data,HeroHolder heroHolder,string rarity)
    {
        heroData = Resources.Load<HeroScriptable>("HeroDatas/" + rarity + "/" + data.heroName);

        heroName = data.heroName;
        heroRarity = (Rarity)Enum.Parse(typeof(Rarity), rarity);
        baseAtk = data.heroAtk;
        attackRange = data.heroAtkRange;
        attackSpeed = data.heroAtkSpeed;
        this.heroHolder = heroHolder;

        circleSpriteRenderer.color = circleColors[(int)heroRarity];

        GetInitEntity(data.heroName,rarity);
        
        InitEquipment(data.leftEquipmentName,data.rightEquipmentName);

        Instantiate(spawnParticle, heroHolder.transform.position,Quaternion.identity);
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

    private void InitEquipment(string leftEuipmentName, string rightEquipmentName)
    {
        if (leftEuipmentName != string.Empty)
        { 
            GameObject leftEquipment = Instantiate(Resources.Load<GameObject>("HeroEquipments/" + leftEuipmentName));
            leftEquipment.transform.SetParent(leftHand);
            leftEquipment.transform.localPosition = Vector3.zero;
        }

        if (rightEquipmentName != string.Empty)
        {
            GameObject rightEquipment = Instantiate(Resources.Load<GameObject>("HeroEquipments/" + rightEquipmentName));
            rightEquipment.transform.SetParent(rightHand);
            rightEquipment.transform.localPosition = Vector3.zero;
        }
    }


    private void Update()
    {
        if (isMove) return;
        CheckForEnemies();
    }

    public void SwapPosition(HeroHolder target, Vector2[] targetPos,int index)
    {
        isMove = true;
        AnimatorChange("MOVE",false);

        if (IsServer)
            transform.parent = target.transform;

        heroHolder = target;

        int sign = (int)Mathf.Sign(targetPos[index].x - transform.position.x);

        switch (sign)
        {
            case -1:
                transform.Rotate(0,180,0);
                break;
            case 1:
                transform.Rotate(0,-180,0);
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

            Vector2 lerpPos = Vector2.Lerp(startPos,endPos,percent);
            transform.position = lerpPos;
            yield return null;
        }

        isMove = false;
        transform.Rotate(0, 180, 0);
        AnimatorChange("IDLE", false);
    }

    private void CheckForEnemies()
    {
        Collider2D[] enemiesInRange =
            Physics2D.OverlapCircleAll(heroHolder.transform.position,attackRange, monsterLayer);

        attackSpeed += Time.deltaTime;

        if (enemiesInRange.Length > 0)
        {
            target = enemiesInRange[0].transform.GetComponent<NetworkObject>();
            if (attackSpeed >= 1.0f)
            {
                attackSpeed = 0.0f;
                AnimatorChange("ATTACK", true);
                LoadBullet();
            }
        }
        else
            target = null;
    }

    public void LoadBullet()
    {
        if (heroData.projectile == null) return;
        Projectile go = Instantiate(heroData.projectile,transform.position,Quaternion.identity);
        go.Init(target.transform,this);
    }

    public void AttackMonster()
    {
        if (target != null)
        {
            AttackMonsterServerRpc(target.NetworkObjectId);
            
            if (UnityEngine.Random.value <= slowChance)
            {
                float[] values = { 1, slowAmount, slowDuration };
                target.GetComponent<Monster>().DebuffServerRpc(
                    (int)DebuffType.SLOW, values
                    );
            }
            else if (UnityEngine.Random.value <= stunChance)
            {
                float[] values = { 3f };
                target.GetComponent<Monster>().DebuffServerRpc(
                    (int)DebuffType.STUN, values
                    );
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AttackMonsterServerRpc(ulong monsterId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(monsterId,out NetworkObject networkObject))
        {
            Monster monster = networkObject.GetComponent<Monster>();
            if (monster != null)
            {
                monster.GetDamage(atk);
            }
        }
    }
}

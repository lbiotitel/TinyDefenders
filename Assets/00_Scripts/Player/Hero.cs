using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Hero : Entity
{
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private HeroScriptable heroData;
    private HeroHolder heroHolder;
    private bool isMove = false;

    [field: SerializeField] public int atk { get; set; }
    [field:SerializeField] public float attackRange { get; set; } = 1f;
    [field: SerializeField] public float attackSpeed { get; private set; } = 1.0f;
    [field: SerializeField] public LayerMask monsterLayer { get; private set; }
    public NetworkObject target { get; set; } = null;

    public override void Awake()
    {
        base.Awake();
    }

    public void InitHeroData(HeroData data,HeroHolder heroHolder,string rarity)
    {
        atk = data.heroAtk;
        attackRange = data.heroAtkRange;
        attackSpeed = data.heroAtkSpeed;
        this.heroHolder = heroHolder;
        GetInitEntity(data.heroName,rarity);
        InitEquipment(data.leftEquipmentName,data.rightEquipmentName);
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
                AttackMonsterServerRpc(target.NetworkObjectId);
            }
        }
        else
            target = null;
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

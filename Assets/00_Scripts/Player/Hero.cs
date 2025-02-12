using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Hero : Entity
{
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    [field: SerializeField] public int atk { get; set; }
    [field:SerializeField] public float attackRange { get; set; } = 1f;
    [field: SerializeField] public float attackSpeed { get; private set; } = 1.0f;
    [field: SerializeField] public LayerMask monsterLayer { get; private set; }
    public NetworkObject target { get; set; } = null;
    [SerializeField] private HeroScriptable heroData;


    public override void Awake()
    {
        base.Awake();
    }

    public void InitHeroData(HeroData data)
    {
        atk = data.heroAtk;
        attackRange = data.heroAtkRange;
        attackSpeed = data.heroAtkSpeed;
        GetInitEntity(data.heroName);
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
        CheckForEnemies();
    }

    private void CheckForEnemies()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position,attackRange, monsterLayer);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

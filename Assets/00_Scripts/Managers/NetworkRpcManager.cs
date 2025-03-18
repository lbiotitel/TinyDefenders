using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkRpcManager : NetworkBehaviour
{
    public static NetworkRpcManager instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    #region About Attack and Debuff
    public void AttackMonster(bool isServer,double damage,Monster monster = null,List<DebuffEffect> debuffList = null)
    {
        if (monster != null)
        {
            AttackMonsterServerRpc(monster.NetworkObjectId, isServer, damage);
            if(debuffList != null)DebuffEffect(isServer, monster, debuffList);
        }
    }

    private void DebuffEffect(bool isServer, Monster monster, List<DebuffEffect> debuffList)
    {
        for (int i = 0; i < debuffList.Count; i++)
        {
            DebuffEffect debuffEffect = debuffList[i];
            DebuffType debuffType = debuffEffect.debuffType;
            float chance = debuffEffect.values[0];

            List<float> values = new List<float>(debuffEffect.values);
            values.RemoveAt(0);

            float[] sendValues = values.ToArray();

            if (UnityEngine.Random.value <= chance)
            {
                if (monster == null) return;
                monster.GetComponent<Monster>().DebuffServerRpc(
                    (int)debuffType, sendValues, isServer
                    );
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AttackMonsterServerRpc(ulong monsterId, bool isServer, double damage)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(monsterId, out NetworkObject networkObject))
        {
            if (networkObject == null) return;
            Monster monster = networkObject.GetComponent<Monster>();
            if (monster != null)
            {
                monster.GetDamage(damage, isServer);
            }
        }
    }

    #endregion



    #region About Spell Card Rpc
    [ServerRpc(RequireOwnership = false)]
    public void SpawnCardAblilityServerRpc(Vector3 targetPos, string cardName)
    {
        SpawnCardAblilityClientRpc(targetPos,cardName);
    }

    [ClientRpc]
    private void SpawnCardAblilityClientRpc(Vector3 targetPos, string cardName)
    {
        AttackCardSO cardSO = SoDataManager.instance.GetSpellCardScirptableObject(cardName) as AttackCardSO;
        CardAbilityBase cardAbilityBase = Instantiate(cardSO.activePrefab, targetPos, Quaternion.identity);
        cardAbilityBase.InitCardAbility(cardSO);
        cardAbilityBase.ActiveAbility();
    }
    #endregion
}

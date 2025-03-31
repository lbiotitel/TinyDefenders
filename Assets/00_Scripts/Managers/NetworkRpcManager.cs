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
    public void AttackMonster(PlayerNetworkType playerNetworkType,double damage, Hero hero,Monster monster = null,List<DebuffEffect> debuffList = null)
    {
        if (monster != null)
        {
            AttackMonsterServerRpc(monster.NetworkObjectId,hero.NetworkObjectId, playerNetworkType, damage);
            if(debuffList != null)DebuffEffect(playerNetworkType, monster, debuffList);
        }
    }

    private void DebuffEffect(PlayerNetworkType playerNetworkType, Monster monster, List<DebuffEffect> debuffList)
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
                    (int)debuffType, sendValues, playerNetworkType
                    );
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AttackMonsterServerRpc(ulong monsterId,ulong heroId, PlayerNetworkType playerNetworkType, double damage)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(monsterId, out NetworkObject networkObject))
        {
            if (networkObject == null) return;
            Monster monster = networkObject.GetComponent<Monster>();
            if (monster != null)
            {
                monster.GetDamage(damage, playerNetworkType, heroId);
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


    #region About Hero Data


    [ServerRpc(RequireOwnership = false)]
    public void GetServerHeroDamageStackServerRpc(ulong heroNetworkId)
    {
        if (NetUtils.TryGetSpawnedObject(heroNetworkId, out NetworkObject heroNetWorkObject))
        {
            Hero hero = heroNetWorkObject.GetComponent<Hero>();
            GetServerHeroKillCountClientRpc(heroNetworkId, hero.GetDamageStack());
        }
    }

    [ClientRpc]
    public void GetServerHeroKillCountClientRpc(ulong heroNetworkId, double damageStack)
    {
        if (IsClient)
        {
            if (NetUtils.TryGetSpawnedObject(heroNetworkId, out NetworkObject heroNetWorkObject))
            {
                Hero hero = heroNetWorkObject.GetComponent<Hero>();
                hero.SetMonsterKillCount(damageStack);
            }
        }
    }


    #endregion


    #region Hero Upgrade


    [ServerRpc(RequireOwnership = false)]
    public void UpgradeHeroServerRpc(int heroTribe, bool isServer)
    {
        HeroTribe tribe = (HeroTribe)heroTribe;

        if (NetUtils.IsHost())
        {
            if (isServer)
            {
                PlayerStatManager.instance.serverUpgrade.Upgrade(tribe);
                InGameUI.instance.RedrawUpgradeUI();
            }
            else
            {

                PlayerStatManager.instance.clientUpgrade.Upgrade(tribe);

                int[] upgradeCosts = new int[(int)HeroTribe.LAST];
                int[] upgradeLevels = new int[(int)HeroTribe.LAST];

                for (int tribeIndex = 0; tribeIndex < (int)HeroTribe.LAST; tribeIndex++)
                {
                    upgradeCosts[tribeIndex] = PlayerStatManager.instance.clientUpgrade.GetHeroUpgradeCost((HeroTribe)tribeIndex);
                    upgradeLevels[tribeIndex] = PlayerStatManager.instance.clientUpgrade.GetHeroUpgradeLevel((HeroTribe)tribeIndex);
                }

                SetUpgradeHeroClientRpc(upgradeLevels, upgradeCosts);
            }
        }
    }

    [ClientRpc]
    public void SetUpgradeHeroClientRpc(int[] upgradeLevels, int[] upgradeCosts)
    {
        for (int tribeIndex = 0; tribeIndex < (int)HeroTribe.LAST; tribeIndex++)
        {
            PlayerStatManager.instance.clientUpgrade.SetHeroUpgradeLevel((HeroTribe)tribeIndex, upgradeLevels[tribeIndex]);
            PlayerStatManager.instance.clientUpgrade.SetHeroUpgradeCost((HeroTribe)tribeIndex, upgradeCosts[tribeIndex]);
        }
        InGameUI.instance.RedrawUpgradeUI();
    }

    #endregion


    #region About Hero Synergy Data

    [ClientRpc]
    public void GetHHeroSynergyDatasClientRpc(int[] heroTribe, int[] synergyLevel)
    {
        for (int i = 0; i < heroTribe.Length; i++)
        {
            PlayerStatManager.instance.SetClientHeroSynergyData(NetUtils.GetPlayerNetworkType(), (HeroTribe)heroTribe[i], synergyLevel[i]);
        }
    }

    #endregion
}

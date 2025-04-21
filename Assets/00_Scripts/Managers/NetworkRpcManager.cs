using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
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

    [SerializeField] private GameFinishStatGameObject gameFinishStatPrefab;

    #region About Attack and Debuff

    public void AttackMonster(PlayerNetworkType playerNetworkType, double damage,Monster monster,List<DebuffEffect> debuffList = null)
    {
        if (monster != null)
        {
            AttackMonsterServerRpc(playerNetworkType,monster.NetworkObjectId,damage);
            if (debuffList != null) DebuffEffect(playerNetworkType, monster, debuffList);
        }
    }

    public void AttackMonster(PlayerNetworkType playerNetworkType, double damage, Hero hero, Monster monster = null, List<DebuffEffect> debuffList = null, bool eventCall = true)
    {
        if (monster != null)
        {
            AttackMonsterServerRpc(monster.NetworkObjectId,hero.NetworkObjectId, playerNetworkType, damage,eventCall);
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
    private void AttackMonsterServerRpc(ulong monsterId,ulong heroId, PlayerNetworkType playerNetworkType, double damage,bool eventCall = true)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(monsterId, out NetworkObject networkObject))
        {
            if (networkObject == null) return;
            Monster monster = networkObject.GetComponent<Monster>();
            if (monster != null)
            {
                monster.GetDamage(damage, playerNetworkType, heroId, eventCall);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AttackMonsterServerRpc(PlayerNetworkType playerNetworkType,ulong monsterId,double damage)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(monsterId, out NetworkObject networkObject))
        {
            if (networkObject == null) return;
            Monster monster = networkObject.GetComponent<Monster>();
            if (monster != null)
            {
                monster.GetDamage(damage, playerNetworkType);
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
        AttackCardSO cardSO = GameResourceManager.instance.GetSpellCardScirptableObject(cardName) as AttackCardSO;
        CardAbilityBase cardAbilityBase = Instantiate(cardSO.activePrefab, targetPos, Quaternion.identity);
        cardAbilityBase.InitCardAbility(cardSO);
        cardAbilityBase.ActiveAbility();
    }
    #endregion


    #region About Upgrade


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

                int[] upgradeCosts = new int[(int)HeroTribe.SCAVENGER + 1];
                int[] upgradeLevels = new int[(int)HeroTribe.SCAVENGER + 1];

                for (int tribeIndex = (int)HeroTribe.HUMAN; tribeIndex <= (int)HeroTribe.SCAVENGER; tribeIndex++)
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
        for (int tribeIndex = (int)HeroTribe.HUMAN; tribeIndex <= (int)HeroTribe.SCAVENGER; tribeIndex++)
        {
            PlayerStatManager.instance.clientUpgrade.SetHeroUpgradeLevel((HeroTribe)tribeIndex, upgradeLevels[tribeIndex]);
            PlayerStatManager.instance.clientUpgrade.SetHeroUpgradeCost((HeroTribe)tribeIndex, upgradeCosts[tribeIndex]);
        }
        InGameUI.instance.RedrawUpgradeUI();
    }


    #endregion


    #region About Hero Synergy 

    [ClientRpc]
    public void GetHeroSynergyDatasClientRpc(FixedString32Bytes[] synergyName, int[] heroCount, int[] synergyLevel)
    {
        for (int i = 0; i < synergyName.Length; i++)
        {
            string synergyNameString = synergyName[i].ToString();   
            PlayerStatManager.instance.SetClientHeroSynergyData(NetUtils.GetPlayerNetworkType(), synergyNameString, heroCount[i], synergyLevel[i]);
        }
    }


    [ClientRpc]
    public void SpawnSynergyParticleClientRpc(string synergyName,Vector3 targetPos)
    {
        HeroSynergySOBase heroSynergySO = GameResourceManager.instance.GetHeroSynergySO(synergyName);
        GameObject particlePrefab = heroSynergySO.synergyBase.GetSynergyParticle();

        GameObject go = PoolingManager.instance.GetObject(particlePrefab, targetPos);
    }


    #endregion



    #region About Game Over Data


    [ClientRpc]
    public void GameFinishClientRpc(string playerName,string friendName,double playerDamage, double friendDamage,int bestWave)
    {
        CreateGameResultStateGameObject(playerName,friendName,playerDamage,friendDamage,bestWave);
        InGameUI.instance.ShowGameOverUI();
    }

    private void CreateGameResultStateGameObject(string playerName, string friendName, double playerDamage, double friendDamage, int bestWave)
    {
        GameFinishStatGameObject gameFinishStatGameObject = Instantiate(gameFinishStatPrefab);
        gameFinishStatGameObject.playerName = playerName;
        gameFinishStatGameObject.friendName = friendName;
        gameFinishStatGameObject.playerDamage = playerDamage;
        gameFinishStatGameObject.friendDamage = friendDamage;
        gameFinishStatGameObject.bestWave = bestWave;
    }

    #endregion


    #region About Player Stat Data

    [ServerRpc(RequireOwnership = false)]
    public void AddAtkModifireServerRpc(int playerNetworkType, float value)
    { 
        PlayerStatManager.instance.AddAtkModifire((PlayerNetworkType)playerNetworkType, value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddAtkSpeedModifireServerRpc(int playerNetworkType, float value)
    {
        PlayerStatManager.instance.AddAtkSpeedModifire((PlayerNetworkType)playerNetworkType, value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddAtkRangeServerRpc(int playerNetworkType, float value)
    {
        PlayerStatManager.instance.AddAtkRangeModifire((PlayerNetworkType)playerNetworkType, value);
    }


    [ServerRpc(RequireOwnership = false)]
    public void AddCriticalRateModifireServerRpc(int playerNetworkType, float value)
    {
        PlayerStatManager.instance.AddCriticalRateModifire((PlayerNetworkType)playerNetworkType, value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddCriDamageModifireServerRpc(int playerNetworkType, float value)
    {
        PlayerStatManager.instance.AddCriDamageModifire((PlayerNetworkType)playerNetworkType, value);
    }



    #endregion


}

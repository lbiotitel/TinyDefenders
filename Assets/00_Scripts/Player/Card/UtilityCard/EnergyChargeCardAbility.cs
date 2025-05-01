using System.Collections.Generic;
using UnityEngine;

public class EnergyChargeCardAbility : CardAbilityBase
{
    public override void ActiveAbility()
    {

        UtilityCardSO utilityCardSO = cardSO as UtilityCardSO;

        float energyChargeValue = utilityCardSO.values[0];
        List<Hero> heroList = SpawnManager.instance.GetHeroList(NetUtils.GetPlayerNetworkType());

        foreach (Hero hero in heroList)
        {
            if (hero != null)
            {
                PoolingManager.instance.GetObject(cardParticlePrefab, hero.transform.position);
                hero.skillContainer.SetSkillGageServerRpc(energyChargeValue);
            }
        }

        Destroy(gameObject);
    }

}

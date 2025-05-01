using System.Collections.Generic;
using UnityEngine;

public class SteamPackCardAbility : CardAbilityBase
{
    public override void ActiveAbility()
    {

        UtilityCardSO utilityCardSO = cardSO as UtilityCardSO;

        float atkModifireValue = utilityCardSO.values[0];
        List<Hero> heroList = SpawnManager.instance.GetHeroList(NetUtils.GetPlayerNetworkType());

        foreach (Hero hero in heroList)
        {
            if (hero != null)
            {
                PoolingManager.instance.GetObject(cardParticlePrefab, hero.transform.position);
                hero.GetHeroData().
                    AddStatModifire(HeroStatType.AtkSpeed, atkModifireValue);
            }
        }

        Destroy(gameObject);
    }
}

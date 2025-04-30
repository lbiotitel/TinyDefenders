using System.Collections.Generic;

public class RageCardAbility : CardAbilityBase
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
                    AddStatModifire(HeroStatType.Atk, atkModifireValue);
            }
        }

        Destroy(gameObject);
    }
}

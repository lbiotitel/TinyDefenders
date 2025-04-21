using UnityEngine;

public class RelicGoldenTrumpet : RelicBase
{
    [SerializeField] private float summonChance;

    public override void ActiveRelic()
    {
        EventManager.instance.onHeroSummonAction += SummonHero;
    }


    private void SummonHero(Hero hero)
    {
        PlayerNetworkType playerNetworkType = NetUtils.GetPlayerNetworkType();

        if (hero.heroOwnerNetworkType != playerNetworkType) return;

        Debug.Log("Alaram 트럼펫 효과 발동");

        MathUtils.RandomChanceAction(summonChance, () =>
        {
            SpawnManager.instance.Spawn(hero.heroName);
        });
    }
}

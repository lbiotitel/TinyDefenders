using UnityEngine;

public class VillagerMerchantSkill : PassiveSkillBase
{
    private int minGoldAmount = 1;
    private int maxGoldAmount = 10;

    public override void SkillLogic()
    {
        base.SkillLogic();

        PlayerNetworkType heroNetworkType = hero.heroOwnerNetworkType;

        GameObject particle = hero.GetHeroData().skillData.skillPrefab;

        PoolingManager.instance.GetObject(particle, transform.position);

        GameManager.instance.GetGold(Random.Range(minGoldAmount,maxGoldAmount),heroNetworkType);
    }
}

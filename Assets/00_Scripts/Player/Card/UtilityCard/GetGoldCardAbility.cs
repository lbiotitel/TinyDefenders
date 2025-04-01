using UnityEngine;

public class GetGoldCardAbility : CardAbilityBase
{
    [SerializeField] private GameObject cardParticle;
    public override void ActiveAbility()
    {
        PoolingManager.instance.GetObject(cardParticle, transform.position);

        UtilityCardSO utilityCardSO = cardSO as UtilityCardSO;

        int minGold = utilityCardSO.values[0];
        int maxGold = utilityCardSO.values[1];    

        int randomGold = Random.Range(minGold, maxGold + 1);

        GameManager.instance.GetGold(randomGold);

        Destroy(gameObject);
    }
}

using UnityEngine;

public class GoldCardAbility : CardAbilityBase
{
    public override void ActiveAbility()
    {
        UtilityCardSO utilityCardSO = cardSO as UtilityCardSO;

        int minGold = utilityCardSO.values[0];
        int maxGold = utilityCardSO.values[1];    

        int randomGold = Random.Range(minGold, maxGold + 1);

        GameManager.instance.GetGold(randomGold);

        Destroy(gameObject);
    }
}

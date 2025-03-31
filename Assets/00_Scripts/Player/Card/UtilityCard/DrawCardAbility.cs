public class DrawCardAbility : CardAbilityBase
{

    public override void ActiveAbility()
    {
        UtilityCardSO utilityCardSO = cardSO as UtilityCardSO;

        for (int i = 0; i < utilityCardSO.values[0]; i++)
        {
            CardManager.instance.DrawCard();
        }

        CardManager.instance.SortCard();

        Destroy(gameObject);
    }
}

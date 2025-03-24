using UnityEngine;

public class UtilityCardNode : CardNodeBase
{
    private UtilityCardSO utilityCardSO;

    public override void InitCardNode(CardSO cardSO)
    {
        base.InitCardNode(cardSO);
        utilityCardSO = cardSO as UtilityCardSO;
    }

    protected override void UseCard()
    {
        utilityCardSO.UseCard();
        base.UseCard();
    }
}

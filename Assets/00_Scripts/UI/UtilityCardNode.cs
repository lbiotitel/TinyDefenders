using UnityEngine;

public class UtilityCardNode : CardNodeBase
{
    private UtilityCardSO utilityCardSO;

    public override void InitCardNode(CardSO cardSO)
    {
        base.InitCardNode(cardSO);
        utilityCardSO = cardSO as UtilityCardSO;

        InitCardRarityBackgroundImage(cardSO.cardRarity);
    }

    protected override void UseCard()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        utilityCardSO.UseCard(worldPos);
        base.UseCard();
    }
}

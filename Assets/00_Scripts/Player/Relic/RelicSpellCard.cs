using UnityEngine;

public class RelicSpellCard : RelicBase
{
    public override void ActiveRelic()
    {
        EventManager.instance.onDrawCardAction += DrawSpellCard;
    }

    private void DrawSpellCard ()
    {
        CardManager.instance.DrawCard(CardType.ATTACK);
    }
}

using UnityEngine;

public class RelicInvitation : RelicBase
{
    public override void ActiveRelic()
    {
       EventManager.instance.onDrawCardAction += DrawHeroCard;
    }

    private void DrawHeroCard()
    {
        CardManager.instance.DrawCard(CardType.SUMMON);
    }    
}

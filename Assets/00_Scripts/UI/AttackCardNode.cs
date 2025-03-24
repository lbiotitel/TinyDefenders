using UnityEngine;
using UnityEngine.EventSystems;

public class AttackCardNode : CardNodeBase
{

    private AttackCardSO attackCardSO;
    public override void InitCardNode(CardSO cardSO)
    {
        base.InitCardNode(cardSO);

        attackCardSO = cardSO as AttackCardSO;
    }


    protected void Update()
    {
        if (isDrag)
        {
            if (Input.mousePosition == transform.position)
            {
                if (cardSO.cardType == CardType.ATTACK)
                {
                    ShowRange();
                    return;
                }
            }
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        HideRange();
    }

    protected void ShowRange()
    {
        AttackCardSO attackCardSO = cardSO as AttackCardSO;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        InGameUI.instance.ShowCardRangeUI(worldPos, attackCardSO.atkRange);
    }

    protected void HideRange()
    {
        InGameUI.instance.HideCardRangeUI();
    }



    protected override void UseCard()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        attackCardSO.UseCard(worldPos);
        base.UseCard();
    }
}

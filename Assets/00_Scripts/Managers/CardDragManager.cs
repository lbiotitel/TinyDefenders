using UnityEngine;

public class CardDragManager : MonoBehaviour
{
    public CardDragManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private CardNode cardNode = null;





    public void SetCardNode(CardNode cardNode)
    { 
        this.cardNode = cardNode;
    }
}

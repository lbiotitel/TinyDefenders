using UnityEngine;

[CreateAssetMenu(fileName = "CardSO", menuName = "Scriptable Objects/CardSO")]
public class CardSO : ScriptableObject
{
    public CardType cardType;
    public int cardCost;
    public string cardName;
    [TextArea]public string cardDescription;
    public Sprite cardImage;

    protected virtual void OnEnable()
    {
        cardName = name;
    }

    protected virtual void Init() { }

}

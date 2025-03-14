using UnityEngine;

[CreateAssetMenu(fileName = "CardSO", menuName = "Scriptable Objects/CardSO")]
public class CardSO : ScriptableObject
{
    public CardType cardType;
    public int cardCost;
    public string cardName;
    public string cardDescription;
    public Sprite cardImage;

}

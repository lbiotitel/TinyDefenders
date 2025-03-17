using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellCardCollectionUINode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private Image cardImage;
    [SerializeField] private TextMeshProUGUI cardCostText;
    [SerializeField] private TextMeshProUGUI cardDescText;

    public void Init(string cardName,Sprite cardImage, string cardDesc,int cardCost)
    {
        this.cardNameText.text = cardName;
        this.cardImage.sprite = cardImage;
        this.cardDescText.text = cardDesc;
        this.cardCostText.text = cardCost.ToString();
    }
}

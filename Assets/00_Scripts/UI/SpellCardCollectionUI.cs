using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SpellCardCollectionUI : MonoBehaviour
{
    [SerializeField] private SpellCardCollectionUINode spellCardUINode;
    [SerializeField] private Transform contentParent;
    List<SpellCardCollectionUINode> cardUINodeList = new List<SpellCardCollectionUINode>();


    private void OnEnable()
    {
        InitAllSpellCard();
    }

    private void InitAllSpellCard()
    {
        if (cardUINodeList.Count > 0)
        {
            cardUINodeList.ForEach(x => Destroy(x.gameObject));
            cardUINodeList.Clear();
        }

        List<CardSO> cardList = SoDataManager.instance.GetAllCardScriptableObjectData();

        foreach (var data in cardList)
        {
            SpellCardCollectionUINode cardNode = Instantiate(spellCardUINode,contentParent);
            cardNode.Init(data.cardName,data.cardImage,data.cardDescription,data.cardCost);

            cardUINodeList.Add(cardNode);
        }
    }
}

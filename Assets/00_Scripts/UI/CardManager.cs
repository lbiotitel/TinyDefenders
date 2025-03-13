using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    [SerializeField] private CardNode cardPrefab;
    [SerializeField] private Transform myCardPos;
    [SerializeField] private float curve;
    [SerializeField] private float cardSpacing;
    private List<CardNode> myCards = new List<CardNode>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DrawCard();
            SortCard();
        }
    }

    public CardNode DrawCard()
    {
        CardNode cardNode = Instantiate(cardPrefab);
        cardNode.transform.SetParent(myCardPos);
        myCards.Add(cardNode);
        return cardNode;
    }

    public void RemoveCard(CardNode card)
    { 
         myCards.Remove(card);
        Destroy(card.gameObject);
    }

    public void SortCard()
    {
        if (myCards.Count == 0) return;

        for (int i = 0; i < myCards.Count; i++)
        {
            if (myCards[i].IsDrag()) continue;

            float pos = (float)i / (myCards.Count - 1);
            if (myCards.Count == 1)
                pos = 0.5f;

            float x = cardSpacing * myCards.Count * (pos - 0.5f);
            float y = Mathf.Sqrt(Mathf.Abs(curve * curve - x * x));
            float rot = 90 - Mathf.Atan2(y, x) * Mathf.Rad2Deg;

            if (pos >= 0)
                rot *= -1;

            RectTransform cardTransform = myCards[i].GetComponent<RectTransform>();
            cardTransform.DOAnchorPos(new Vector2(x, y - curve), 0.2f);
            cardTransform.DORotate(new Vector3(0, 0, rot),0.2f);

            myCards[i].transform.SetSiblingIndex(i);
            myCards[i].InitCardNode();
        }
    }



}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    [SerializeField] private CardNode cardPrefab;
    [SerializeField] private Transform myCardPos;
    [SerializeField] private float curve;
    [SerializeField] private float cardSpacing;

    [SerializeField] private int rerollHeroCount;
    [SerializeField] private int rerollSpellCardCount;
    private List<CardNode> myCards = new List<CardNode>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RerollCard();
        }
    }
#endif



    public void RerollCard()
    {
        for (int i = 0; i < rerollHeroCount; i++)
        {
            DrawCard(CardType.SUMMON);
        }

        for (int i = 0; i < rerollSpellCardCount; i++)
        {
            DrawCard(CardType.ATTACK);
        }

        SortCard();
    }


    private void DrawCard(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.SUMMON:
                InstantiateCard(GetSummonCardSO());
                break;
            case CardType.ATTACK:
                InstantiateCard(GetRandomCardSO());
                break;
        }
    }

    private void InstantiateCard(CardSO cardSO)
    {
        CardNode cardNode = Instantiate(cardPrefab);
        cardNode.InitCardNode(cardSO);
        cardNode.transform.SetParent(myCardPos);
        myCards.Add(cardNode);
    }

    private CardSO GetRandomCardSO()
    {
        CardRarity randomRarity = (CardRarity)Random.Range(0,(int)CardRarity.Rare);
        List<CardSO> cardList = SoDataManager.instance.GetAllSpellCardScirptableObject(CardRarity.Normal);

        int randomCard = Random.Range(0, cardList.Count);

        return cardList[randomCard];
    }

    private SummonCard GetSummonCardSO()
    {
        HeroScriptable heroData = Spawner.instance.LoadData(Utils.GetIngameSummonRarity().ToString());
        SummonCard cardSO = ScriptableObject.CreateInstance<SummonCard>();
        cardSO.InitSummonCard(heroData);

        return cardSO;
    }

    public void RemoveCard(CardNode card)
    {
        myCards.Remove(card);
        Destroy(card.gameObject);
    }

    public void SortCard()
    {
        if (myCards.Count == 0)
        {
            myCards.Clear();
            RerollCard();
        }

        StartCoroutine(SortCardCoroutine());
    }

    private IEnumerator SortCardCoroutine()
    {
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
            cardTransform.DORotate(new Vector3(0, 0, rot), 0.2f);

            myCards[i].transform.SetSiblingIndex(i);

            yield return new WaitForSeconds(0.2f);
        }
    }

}

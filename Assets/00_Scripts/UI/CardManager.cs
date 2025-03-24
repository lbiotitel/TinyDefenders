using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    [SerializeField] private SummonCardNode summonCardPrefab;
    [SerializeField] private AttackCardNode attackCardPrefab;
    [SerializeField] private UtilityCardNode utilityCardPrefab;

    [SerializeField] private Transform myCardPos;
    [SerializeField] private Button rerollButton;
    [SerializeField] private GameObject cardDropZone;

    [SerializeField] private int rerollHeroCount;
    [SerializeField] private int rerollSpellCardCount;
    private List<CardNodeBase> myCardList = new List<CardNodeBase>();

    public static int MostFrontCardOrder = 100;
    private float cardSpacing;
    private float cardWidth;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            cardWidth = myCardPos.GetComponent<BoxCollider2D>().bounds.size.x;
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

    private void Start()
    {
        rerollButton.onClick.AddListener(() => RerollCard());
    }

    public void RerollCard()
    {

        if (myCardList.Count > 0)
        {
            while (myCardList.Count > 0)
            {
                RemoveCard(myCardList[0]);
            }

            myCardList.Clear();
        }

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

    public void DrawCard()
    {
        //draw random card
        int random = Random.Range(0, ((int)CardType.UTILITY) - 1);
        DrawCard((CardType)random);
    }

    private void DrawCard(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.SUMMON:
                GetSummonCardSO();
                break;
            case CardType.ATTACK:
                GetRandomCardSO();
                break;
        }
    }

    private void GetRandomCardSO()
    {
        CardRarity randomRarity = (CardRarity)Random.Range(0,(int)CardRarity.Rare);
        List<CardSO> cardList = SoDataManager.instance.GetAllSpellCardScirptableObjectByRarity(CardRarity.Normal);

        int randomCard = Random.Range(0, cardList.Count);

        CardType cardType = cardList[randomCard].cardType;

        CardNodeBase cardNode = null;

        switch (cardType)
        {
            case CardType.ATTACK:
                cardNode = Instantiate(attackCardPrefab, transform.position, Quaternion.identity);
                break;
            case CardType.UTILITY:
                cardNode = Instantiate(utilityCardPrefab, transform.position, Quaternion.identity);
                break;
        }
       
        cardNode.InitCardNode(cardList[randomCard]);
        cardNode.transform.SetParent(myCardPos);
        myCardList.Add(cardNode);
    }

    private void GetSummonCardSO()
    {
        HeroScriptable heroData = Spawner.instance.LoadData(Utils.GetIngameSummonRarity().ToString());
        SummonCard cardSO = ScriptableObject.CreateInstance<SummonCard>();
        cardSO.cardType = CardType.SUMMON;
        cardSO.InitSummonCard(heroData);

        SummonCardNode cardNode = Instantiate(summonCardPrefab, transform.position, Quaternion.identity);
        cardNode.InitCardNode(cardSO);
        cardNode.transform.SetParent(myCardPos);

        myCardList.Add(cardNode);
    }

    public void RemoveCard(CardNodeBase card)
    {
        myCardList.Remove(card);
        Destroy(card.gameObject);
    }

    public void SortCard()
    {
        if (myCardList.Count == 0)
        {
            myCardList.Clear();
            RerollCard();
        }

        StartCoroutine(SortCardCoroutine());
    }


    public void OffCardDropZone()
    {
        cardDropZone.SetActive(false); 
    }

    public void OnCardDropZone()
    { 
        cardDropZone.SetActive(true); 
    }

    private IEnumerator SortCardCoroutine()
    {
        SetCardInteract(false);

        cardSpacing = cardWidth / myCardList.Count;

        for (int i = 0; i < myCardList.Count; i++)
        {
            SoundManager.instance.PlaySfxSound("CardDrawSFX");

            float pos = (float)i / (myCardList.Count - 1);
            if (myCardList.Count == 1)
                pos = 0.5f;

            float x = cardSpacing * myCardList.Count * (pos - 0.5f);
            float y = myCardPos.position.y;

            RectTransform cardTransform = myCardList[i].GetComponent<RectTransform>();
            cardTransform.DOAnchorPos(new Vector2(x, y), 0.1f);

            myCardList[i].transform.SetAsFirstSibling();

            yield return new WaitForSeconds(0.1f);
        }

        SetCardInteract(true);
    }

    private void SetCardInteract(bool interact)
    {
        foreach (var card in myCardList)
        {
            card.InitReadyInteract(interact);
        }
    }

}

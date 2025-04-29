using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    [SerializeField] private HeroCardNode summonCardPrefab;
    [SerializeField] private AttackCardNode attackCardPrefab;
    [SerializeField] private UtilityCardNode utilityCardPrefab;
    [SerializeField] private string cardDrawSFX;

    [SerializeField] private Transform myCardPos;
    
    [SerializeField] private Button rerollButton;
    [SerializeField] private Image rerollButtonImage;
    [SerializeField] private float rerollCoolTime;
    private bool canReroll = true;

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
            RerollCard(true);
        }
    }
#endif

    private void Start()
    {
        OffCardDropZone();
        EventManager.instance.onWaveFinishAction += () => RerollCard(true);
    }


    public void RerollCard(bool cardEmpty)
    {
        if (!cardEmpty)
        {
            if (!canReroll) return;
            int rerollCost = PlayerStatManager.instance.rerollCost;
            if (GameManager.instance.IsEnoughGold(rerollCost))
            {
                canReroll = false;
                DrawCardLogic();
                StartCoroutine(CardRerollCoolTimeCoroutine());
                GameManager.instance.SpendGold(rerollCost);
            }
        }
        else {
            DrawCardLogic();
        }

        SortCard();
    }

    private void DrawCardLogic()
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

        EventManager.instance.onDrawCardAction?.Invoke();
    }

    public void DrawCard()
    {
        //draw random card
        int random = Random.Range(0, ((int)CardType.UTILITY) - 1);
        DrawCard((CardType)random);
    }


    public void DrawCard(string cardName)
    {
        CardSO cardSo = GameResourceManager.instance.GetSpellCardScirptableObject(cardName);

        InitSpellCard(cardSo);
    }

    private void InitSpellCard(CardSO cardSo)
    {
        CardNodeBase cardNode = null;

        switch (cardSo.cardType)
        {
            case CardType.ATTACK:
                cardNode = Instantiate(attackCardPrefab, transform.position, Quaternion.identity);
                break;
            case CardType.UTILITY:
                cardNode = Instantiate(utilityCardPrefab, transform.position, Quaternion.identity);
                break;
        }

        cardNode.InitCardNode(cardSo);
        cardNode.gameObject.SetActive(true);
        cardNode.transform.SetParent(myCardPos, false);

        myCardList.Add(cardNode);
    }

    public void DrawCard(CardType cardType)
    {
        SoundManager.instance.PlaySfxSound(cardDrawSFX, SFXTYPE.UI);
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
        List<CardSO> cardList = GameResourceManager.instance.GetAllSpellCardScirptableObjectByRarity(CardRarity.Normal);

        int randomCard = Random.Range(0, cardList.Count);

        InitSpellCard(cardList[randomCard]);
    }

    private void GetSummonCardSO()
    {
        HeroScriptable heroData = GameResourceManager.instance.GetRandomHeroScriptiableData(PlayerStatManager.instance.ingameLevel);

        HeroCard cardSO = ScriptableObject.CreateInstance<HeroCard>();
        cardSO.cardType = CardType.SUMMON;
        cardSO.InitSummonCard(heroData);

        HeroCardNode cardNode = Instantiate(summonCardPrefab, transform.position, Quaternion.identity);
        cardNode.InitCardNode(cardSO);
        cardNode.gameObject.SetActive(true);
        cardNode.transform.SetParent(myCardPos,false);

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
            RerollCard(true);
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

            float pos = (float)i / (myCardList.Count - 1);
            if (myCardList.Count == 1)
                pos = 0.5f;

            float x = cardSpacing * myCardList.Count * (pos - 0.5f);
            float y = myCardPos.position.y;

            RectTransform cardTransform = myCardList[i].GetComponent<RectTransform>();
            cardTransform.DOAnchorPos(new Vector2(x, y), 0.1f);

            myCardList[i].transform.SetAsLastSibling();

            yield return new WaitForSeconds(0.1f);
        }

        SetCardInteract(true);
    }

    private IEnumerator CardRerollCoolTimeCoroutine()
    {
        float t = 0f;
        while (t < rerollCoolTime)
        {
            t += Time.deltaTime;

            rerollButtonImage.fillAmount = (t/rerollCoolTime);

            yield return null;
        }

        canReroll = true;
    }


    private void SetCardInteract(bool interact)
    {
        foreach (var card in myCardList)
        {
            card.InitReadyInteract(interact);
        }
    }

}

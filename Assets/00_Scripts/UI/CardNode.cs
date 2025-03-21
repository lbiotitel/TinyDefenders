using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardNode : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,
    IBeginDragHandler,IDragHandler,IEndDragHandler
{
    [Header("Basic Card Info")]
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI cardDescText;
    [SerializeField] private Image cardImage;

    [Header("HeroCard Info")]
    [SerializeField] private GameObject heroInfoBackground;
    [SerializeField] private TextMeshProUGUI heroAtkText;
    [SerializeField] private TextMeshProUGUI heroAtkSpeedText;
    [SerializeField] private TextMeshProUGUI heroAtkRangeText;

    private Vector2 enlargementOriginalPosition;

    private Vector2 dragOriginalPosition;

    private Transform parentTransform;
    private Transform dragParent;
    private bool isEnlargement = false;
    private bool isDrag = false;

    private CardSO cardSO;

    private bool readyInteract = false;

    private void Start()
    {
        dragParent = GameObject.Find("CardDrag").transform;
    }

    public void InitCardNode(CardSO cardSO)
    {
        this.cardSO = cardSO;

        cardImage.sprite = cardSO.cardImage;
        cardNameText.text = cardSO.cardName;

        cardImage.preserveAspect = true;

        InitCardEachType();
    }

    private void InitCardEachType()
    {
        if (cardSO.cardType == CardType.SUMMON)
        {
            heroInfoBackground.SetActive(true);
            InitSummonCard();
        }
        else if(cardSO.cardType == CardType.ATTACK)
        {
            cardDescText.text = cardSO.cardDescription;
            heroInfoBackground.SetActive(false);
        }
    }

    private void InitSummonCard()
    { 
        SummonCard summonCard = cardSO as SummonCard;
        HeroScriptable heroData = summonCard.GetHeroData();
        heroAtkText.text = heroData.atk.ToString();
        heroAtkSpeedText.text = heroData.atkSpeed.ToString();
        heroAtkRangeText.text = heroData.atkRange.ToString();
        cardDescText.text = heroData.heroDesc;
    }

    private void Update()
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


    public void InitReadyInteract(bool interact)
    { 
        readyInteract = interact;
        if (readyInteract)
        {
            dragOriginalPosition = transform.position;
            enlargementOriginalPosition = transform.position;
        }
    }


    #region Card Drag Method
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        if (IsInDropZone())
        {
            transform.localScale = Vector3.one * 0.5f;
        }
        else {
            transform.localScale = Vector3.one;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;

        if (!IsInDropZone())
        {
            transform.position = dragOriginalPosition;
            transform.SetParent(parentTransform);
            ReturnOriginalCard();
        }
        else {
            CheckCost();
            CardManager.instance.RemoveCard(this);
        }

        HideRange();
        CardManager.instance.SortCard();
    }

    public void OnBeginDrag(PointerEventData evenData)
    {
        if (!readyInteract) return;
        isDrag = true;
        parentTransform = transform.parent;
        transform.rotation = Quaternion.identity;
        transform.SetParent(dragParent);
    }


    #endregion


    private bool IsInDropZone()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("DropZone"))
            {
                return true;
            }
        }

        return false;
    }

    private void ShowRange()
    {
        AttackCardSO attackCardSO = cardSO as AttackCardSO;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        InGameUI.instance.ShowCardRangeUI(worldPos, attackCardSO.atkRange);
    }

    private void HideRange()
    {
        InGameUI.instance.HideCardRangeUI();
    }


    private void UseCard()
    {
        switch (cardSO.cardType)
        {
            case CardType.SUMMON:
                SummonCard summonCard = cardSO as SummonCard;
                summonCard.Summon(transform);
                break;

            case CardType.ATTACK:
                AttackCardSO activeCardSO = cardSO as AttackCardSO;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPos.z = 0f;
                activeCardSO.UseCard(worldPos);
                break;

            case CardType.UTILITY:
                UtilityCardSO utilityCardSO = cardSO as UtilityCardSO;
                utilityCardSO.UseCard();
                break;
        }
        CardManager.instance.RemoveCard(this);
    }


    private void CheckCost()
    {
        if (GameManager.instance.gold >= cardSO.cardCost)
        { 
            GameManager.instance.gold -= cardSO.cardCost;
            UseCard();
        }
        else
        {
            //재화가 부족하다는 알림
        }
    }


    #region Mouse in Card
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!readyInteract) return;
        if (isDrag) return;
        if (!isEnlargement)
        {
            EnlargeMentCard();
        }
    }

    private void EnlargeMentCard()
    {
        isEnlargement = true;
        transform.SetAsLastSibling();
        transform.DOMove(new Vector3(transform.position.x, transform.position.y + 300f), 0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDrag) return;
        if (isEnlargement)
        {
            ReturnOriginalCard();
        }
    }

    private void ReturnOriginalCard()
    {
        transform.DOMove(enlargementOriginalPosition, 0.2f).OnComplete(() => {
            isEnlargement = false;
        });
    }

    #endregion

    public bool IsDrag() { return isDrag; }

}

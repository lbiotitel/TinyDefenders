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
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private Image cardImage;

    private Vector2 enlargementOriginalPosition;

    private Vector2 dragOriginalPosition;
    private Quaternion dragOriginalQuaternion;

    private Transform parentTransform;
    private Transform dragParent;
    private bool isEnlargement = false;
    private bool isDrag = false;

    private CardSO cardSO;

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
            transform.rotation = dragOriginalQuaternion;
            transform.SetParent(parentTransform);
            ReturnOriginalCard();
        }
        else {
            UseCard();
            CardManager.instance.RemoveCard(this);
        }

        HideRange();
        CardManager.instance.SortCard();
    }

    public void OnBeginDrag(PointerEventData evenData)
    {
        isDrag = true;
        dragOriginalPosition = transform.position;
        dragOriginalQuaternion = transform.rotation;
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



    #region Mouse in Card
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDrag) return;
        if (!isEnlargement)
        {
            EnlargeMentCard();
        }
    }

    private void EnlargeMentCard()
    {
        enlargementOriginalPosition = transform.position;
        transform.DOMove(new Vector2(transform.position.x, transform.position.y + 300f), 0f).OnComplete(() =>
        {
            isEnlargement = true;
        });
        transform.rotation = Quaternion.identity;
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
        transform.DOMove(enlargementOriginalPosition, 0f).OnComplete(() => {
            isEnlargement = false;
        });
    }

    #endregion

    public bool IsDrag() { return isDrag; }

}

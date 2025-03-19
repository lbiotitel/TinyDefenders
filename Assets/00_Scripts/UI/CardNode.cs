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
    private Quaternion enlargementOriginalQuaternion;

    private Vector2 dragOriginalPosition;
    private Quaternion dragOriginalQuaternion;

    private Transform parentTransform;
    private Transform dragParent;
    private bool isInit = false;
    private bool isEnlargement = false;
    private bool isDrag = false;

    private CardSO cardSO;

    private void Start()
    {
        dragParent = GameObject.Find("CardDrag").transform;
    }

    public void InitCardNode(CardSO cardSO)
    {
        this.isInit = true;
        this.cardSO = cardSO;

        cardImage.sprite = cardSO.cardImage;
        cardNameText.text = cardSO.cardName;

        cardImage.preserveAspect = true;
    }


    #region Card Drag Method
    public void OnDrag(PointerEventData eventData)
    {
        if (!isInit) return;
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
        if (!isInit) return;
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

        CardManager.instance.SortCard();
    }

    public void OnBeginDrag(PointerEventData evenData)
    {
        if (!isInit) return;
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
        ShowRange();
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
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        InGameUI.instance.ShowCardRangeUI(worldPos, 2f);
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
        }
        CardManager.instance.RemoveCard(this);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInit) return;
        if (isDrag) return;
        if (!isEnlargement)
        {
            EnlargeMentCard();
        }
    }

    private void EnlargeMentCard()
    {
        enlargementOriginalPosition = transform.position;
        enlargementOriginalQuaternion = transform.transform.rotation;
        transform.DOScale(new Vector2(1.5f, 1.5f), 0f).OnComplete(() =>
        {
            isEnlargement = true;
        });
        transform.DOMove(new Vector2(transform.position.x, transform.position.y + 300f), 0f);
        transform.rotation = Quaternion.identity;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isInit) return;
        if (isDrag) return;
        if (isEnlargement)
        {
            ReturnOriginalCard();
        }
    }

    private void ReturnOriginalCard()
    {
        transform.DOMove(enlargementOriginalPosition, 0f);
        transform.rotation = enlargementOriginalQuaternion;
        transform.DOScale(new Vector2(1f, 1f), 0f).OnComplete(() => {
            isEnlargement = false;
        });
    }

    public bool IsDrag() { return isDrag; }

}

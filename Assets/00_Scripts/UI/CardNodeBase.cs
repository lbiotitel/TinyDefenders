using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[System.Serializable]
public class CardColorNode {
    public Sprite cardImage;
    public CardRarity CardRarity;
}

public class CardNodeBase : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,
    IBeginDragHandler,IDragHandler,IEndDragHandler
{
    [Header("Basic Card Info")]
    [SerializeField] protected Image backgroundCardImage;
    [SerializeField] protected TextMeshProUGUI cardNameText;
    [SerializeField] protected TextMeshProUGUI cardDescText;
    [SerializeField] protected TextMeshProUGUI cardCostText;
    [SerializeField] protected Image cardImage;

    [SerializeField] protected CardColorNode[] cardColorNode;

    protected Vector2 enlargementOriginalPosition;

    protected Vector2 dragOriginalPosition;
    protected float yHeight;
    protected Transform parentTransform;
    protected Transform dragParent;
    protected bool isEnlargement = false;
    protected bool isDrag = false;

    protected CardSO cardSO;

    protected bool readyInteract = false;

    protected void Start()
    {
        dragParent = GameObject.Find("CardDrag").transform;
    }

    public virtual void InitCardNode(CardSO cardSO)
    {
        cardImage.preserveAspect = true;
        this.cardSO = cardSO;

        cardImage.sprite = cardSO.cardImage;
        cardNameText.text = cardSO.cardName;
        cardDescText.text = cardSO.cardDescription;
        cardCostText.text = cardSO.cardCost.ToString();

        yHeight = GetComponent<RectTransform>().rect.size.y / 2f;
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
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!readyInteract) return;
        CardManager.instance.OnCardDropZone();
        transform.position = Input.mousePosition;
        if (IsInDropZone())
        {
            transform.localScale = Vector3.one * 0.5f;
        }
        else {
            transform.localScale = Vector3.one;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!readyInteract) return;
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

        CardManager.instance.OffCardDropZone();
    }

    public virtual void OnBeginDrag(PointerEventData evenData)
    {
        if (!readyInteract) return;
        isDrag = true;
        parentTransform = transform.parent;
        transform.rotation = Quaternion.identity;
        transform.SetParent(dragParent);
    }


    #endregion


    protected bool IsInDropZone()
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



    protected virtual void UseCard()
    {
        CardManager.instance.RemoveCard(this);
        CardManager.instance.SortCard();
    }


    protected void CheckCost()
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

    protected void EnlargeMentCard()
    {
        isEnlargement = true;
        transform.SetAsLastSibling();
        transform.DOMove(new Vector3(transform.position.x, transform.position.y + yHeight), 0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDrag) return;
        if (isEnlargement)
        {
            ReturnOriginalCard();
        }
    }

    protected void ReturnOriginalCard()
    {
        transform.DOMove(enlargementOriginalPosition, 0.2f).OnComplete(() => {
            isEnlargement = false;
        });
    }

    #endregion

    public bool IsDrag() { return isDrag; }

    protected void InitCardRarityBackgroundImage(CardRarity cardRarity)
    {
        backgroundCardImage.sprite = cardColorNode[(int)cardRarity].cardImage;
    }

}

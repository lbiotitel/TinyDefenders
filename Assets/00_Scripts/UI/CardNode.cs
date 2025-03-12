using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
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
    bool isEnlargement = false;
    bool isDrag = false;


    private void Start()
    {
        dragParent = GameObject.Find("CardDrag").transform;
    }

    public void InitCardNode()
    { 
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        if (IsInDropZone())
        { 
            
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
        CardManager.instance.SortCard();
    }

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

    private void UseCard()
    {
        //카드 효과 발동
        Debug.Log("카드효과 발동");
        CardManager.instance.RemoveCard(this);
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

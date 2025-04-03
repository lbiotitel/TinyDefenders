using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SwipeMapUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private float swipeTime = 0.2f;
    [SerializeField] private float swapDistance = 50.0f;
    [SerializeField] private Transform contentTransform;

    private string[] mapNames;

    private float[] scrollPageCount;
    private float valueDistance = 0;
    private int currentpage = 0;
    private int maxPage = 0;
    
    private float startTouchX;
    private float endTouchX;
    private bool isSwipe = false;

    private void Awake()
    {
        scrollPageCount = new float[contentTransform.childCount];
        mapNames = new string[contentTransform.childCount];

        for (int i = 0; i < mapNames.Length; i++)
        {
            mapNames[i] = contentTransform.GetChild(i).name;
        }

        valueDistance = 1f / (scrollPageCount.Length - 1f);

        for (int i = 0; i < scrollPageCount.Length; i++)
        {
            scrollPageCount[i] = valueDistance * i;
        }

        maxPage = scrollPageCount.Length;
    }

    private void Start()
    {
        SetScrollBarValue(0);
    }

    public void SetScrollBarValue(int index)
    {
        currentpage = index;
        scrollbar.value = scrollPageCount[index];
    }



    private void UpdateSwipe()
    {
        if (isSwipe) return;

        isSwipe = true;
        if (Mathf.Abs(startTouchX - endTouchX) < swapDistance)
        {
            StartCoroutine(SwipeCoroutine(currentpage));
            return;
        }

        bool isLeft = startTouchX < endTouchX;

        if (isLeft)
        {
            currentpage = Mathf.Max(0, --currentpage);
        }
        else
        {
            currentpage = Mathf.Min(maxPage - 1, ++currentpage);
        }

        StartCoroutine(SwipeCoroutine(currentpage));
    }

    private IEnumerator SwipeCoroutine(int index)
    {
        float current = 0;
        float percent = 0;

        while (percent <= swipeTime)
        {
            current += Time.deltaTime;
            percent = current / swipeTime;

            scrollbar.value = Mathf.Lerp(scrollbar.value, scrollPageCount[index], percent);

            yield return null;
        }

        SetScrollBarValue(index);

        isSwipe = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isSwipe) return;
        startTouchX = eventData.position.x;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if (isSwipe) return;
        endTouchX = eventData.position.x;
        UpdateSwipe();
    }
}

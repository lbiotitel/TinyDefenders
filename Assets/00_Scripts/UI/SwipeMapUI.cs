using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwipeMapUI : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private float swipeTime = 0.2f;
    [SerializeField] private float swapDistance = 50.0f;

    private string[] mapNames;

    private float[] scrollPageCount;
    private float valueDistance = 0;
    private int currentpage = 0;
    private int maxPage = 0;
    private float startTouchX;
    private float endTouchX;
    private bool isSwipeMode = false;

    private void Awake()
    {
        scrollPageCount = new float[transform.childCount];
        mapNames = new string[transform.childCount];

        for (int i = 0; i < mapNames.Length; i++)
        {
            mapNames[i] = transform.GetChild(i).name;
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

    private void Update()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        if (isSwipeMode) return;

        if (Input.GetMouseButtonDown(0))
        {
            startTouchX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(0))
        { 
            endTouchX = Input.mousePosition.x;

            UpdateSwipe();
        }
    }

    private void UpdateSwipe()
    {
        if (Mathf.Abs(startTouchX - endTouchX) < swapDistance)
        {
            StartCoroutine(SwipeCoroutine(currentpage));
            return;
        }

        bool isLeft = startTouchX < endTouchX ? true : false;

        if (isLeft)
        {
            currentpage = Mathf.Max(0, --currentpage);
        }
        else {
            currentpage = Mathf.Min(maxPage - 1,++currentpage);
        }

        StartCoroutine(SwipeCoroutine(currentpage));
    }

    private IEnumerator SwipeCoroutine(int index)
    {
        float startValue = scrollbar.value;
        float current = 0;
        float percent = 0;

        isSwipeMode = true;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / swipeTime;

            scrollbar.value = Mathf.Lerp(startValue, scrollPageCount[index],percent);

            yield return null;
        }

        isSwipeMode = false;
    }
}

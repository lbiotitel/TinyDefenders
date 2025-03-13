using System.Collections.Generic;
using UnityEngine;

public class CardArrowGuide : MonoBehaviour
{
    [SerializeField] private GameObject arrowHeadPrefab;
    [SerializeField] private GameObject arrowBodyPrefab;
    [SerializeField] private int arrowBodyCount;
    [SerializeField] private float scaleFactor = 1f;
    [SerializeField] private RectTransform targetPos;

    private RectTransform startPos;

    private List<RectTransform> arrowNode = new List<RectTransform>();
    private List<Vector2> controlPoints = new List<Vector2>();
    private readonly List<Vector2> controlPointFactors = new List<Vector2>
    {
        new Vector2(-0f, -0.5f),  
        new Vector2(0f, -1.2f)   
    };

    private void Awake()
    {
        InitArrow();
    }

    public void ShowArrow(RectTransform startPos)
    {
        gameObject.SetActive(true);
        this.startPos = startPos;
    }

    public void HideArrow()
    {
        startPos = null;
        gameObject.SetActive(false);
    }

    private void InitArrow()
    {

        for (int i = 0; i < arrowBodyCount; i++)
        {
            arrowNode.Add(Instantiate(arrowBodyPrefab, transform).GetComponent<RectTransform>());
        }

        arrowNode.Add(Instantiate(arrowHeadPrefab, transform).GetComponent<RectTransform>());

        arrowNode.ForEach(x => x.GetComponent<RectTransform>().position = new Vector2(-1000f, -1000f));

        for (int i = 0; i < 4; i++)
        {
            controlPoints.Add(Vector2.zero);
        }

    }

    private void Update()
    {
        DrawArrow();
    }

    private void DrawArrow()
    {
        if (startPos == null) return;

        float cardHeight = startPos.rect.height;

        controlPoints[0] = new Vector2(startPos.transform.position.x, startPos.position.y + cardHeight);

        controlPoints[3] = new Vector2(targetPos.transform.position.x, targetPos.transform.position.y);

        controlPoints[1] = controlPoints[0] + (controlPoints[3] - controlPoints[0]) * controlPointFactors[0];
        controlPoints[2] = controlPoints[0] + (controlPoints[3] - controlPoints[0]) * controlPointFactors[1];

        for (int i = 0; i < arrowNode.Count; i++)
        {
            var t = Mathf.Log(1f * i / (arrowNode.Count - 1) + 1f, 2f);

            arrowNode[i].position =
                Mathf.Pow(1 - t, 3) * controlPoints[0] +
                3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1] +
                3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2] +
                Mathf.Pow(t, 3) * controlPoints[3];

            if (i > 0)
            {
                var euler = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, arrowNode[i].position - arrowNode[i - 1].position));
                arrowNode[i].rotation = Quaternion.Euler(euler);
            }

            var scale = scaleFactor * (1f - 0.03f * (arrowNode.Count - 1 - i));
            arrowNode[i].localScale = new Vector3(scale, scale, 1f);
        }

        arrowNode[0].transform.rotation = arrowNode[1].transform.rotation;
    }
}

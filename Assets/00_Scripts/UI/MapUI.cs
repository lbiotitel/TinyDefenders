using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class MapNode {
    public MapType mapType;
    public Sprite mapImage;
}

public class MapUI : MonoBehaviour
{
    [SerializeField] private MapNode[] mapNodes;

    [SerializeField] private TMP_Text mapNameText;
    [SerializeField] private Image mapImage;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    private int currentMapIndex = 0;

    private void Start()
    {
        nextButton.onClick.AddListener(NextMap);
        prevButton.onClick.AddListener(PrevMap);
        SetMapUI();
    }

    private void NextMap()
    {
        currentMapIndex = Mathf.Min(++currentMapIndex, mapNodes.Length - 1);
        SetMapUI();
    }

    private void PrevMap()
    { 
        currentMapIndex = Mathf.Max(--currentMapIndex, 0);
        SetMapUI();
    }

    private void SetMapUI()
    {
        string mapName = SceneName.GetMapName(mapNodes[currentMapIndex].mapType);
        mapNameText.text = mapName;
        mapImage.sprite = mapNodes[currentMapIndex].mapImage;
    }

    public MapType GetMapType()
    { 
        return mapNodes[currentMapIndex].mapType;
    }

    private void OnDestroy()
    {
        nextButton.onClick.RemoveListener(NextMap);
        prevButton.onClick.RemoveListener(PrevMap);
    }
}

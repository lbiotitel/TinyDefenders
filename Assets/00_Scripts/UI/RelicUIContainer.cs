using UnityEngine;
using UnityEngine.UI;

public class RelicUIContainer : MonoBehaviour
{
    [SerializeField] private GameObject relicUINodePrefab;


    private void Start()
    {
        EventManager.instance.onRelicSelectAction += AddRelicUI;
    }


    private void AddRelicUI(RelicSO relicSO)
    {
        GameObject relicUINode = Instantiate(relicUINodePrefab,transform);
        relicUINode.GetComponent<Image>().sprite = relicSO.relicImage;
        relicUINode.SetActive(true);
    }
}

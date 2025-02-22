using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombineNodeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private TextMeshProUGUI heroDesc;
    [SerializeField] private Transform horizontalLayout;


    public void SetName(string name) { heroName.text = name; }

    public void AddHeroUI(GameObject gameObject)
    {
        gameObject.transform.SetParent(horizontalLayout);
    }
}

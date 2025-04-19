using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawCardRateUINode : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI[] probabilityText;

    private void Start()
    {
        InitDrawCardProbability(1);
        EventManager.instance.onLevelChagendAction += InitDrawCardProbability;
    }

    private void InitDrawCardProbability(int level)
    {
        List<float> probabilityList = GameResourceManager.instance.GetDrawHeroCardProbability(level);

        for(int i = 0; i < probabilityText.Length; i++)
        {
            probabilityText[i].text = string.Format("{0:F2}%", (probabilityList[i] * 100f));
        }
    }

}

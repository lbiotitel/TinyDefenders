using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawCardRateUINode : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI[] probabilityText;

    public void InitUpgradeUI()
    {
        InitDrawCardProbability();
    }

    private void InitDrawCardProbability()
    {
        List<float> probabilityList = GameResourceManager.instance.GetDrawHeroCardProbability(0);

        for(int i = 0; i < probabilityText.Length; i++)
        {
            probabilityText[i].text = string.Format("{0:F2}%", (probabilityList[i] * 100f));
        }
    }

}

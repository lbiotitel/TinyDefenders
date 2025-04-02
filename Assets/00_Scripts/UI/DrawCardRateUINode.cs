using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawCardRateUINode : MonoBehaviour
{

    public TextMeshProUGUI upgradeLevelText;
    public TextMeshProUGUI upgradeCostText;
    public Button upgradeButton;
    private int level;
    private int cost;

    [SerializeField] private TextMeshProUGUI[] probabilityText;

    private void Awake()
    {
        upgradeButton.onClick.AddListener(Upgrade);
    }

    public void InitUpgradeUI()
    {
        this.level = PlayerStatManager.instance.GetcCardDrawRateUpgradeLevel(NetUtils.GetPlayerNetworkType());
        this.cost = PlayerStatManager.instance.GetCardDrawRateUpgradeCost(NetUtils.GetPlayerNetworkType());

        upgradeCostText.text = cost.ToString();
        upgradeLevelText.text = "LV" + level.ToString();

        InitDrawCardProbability();
    }

    private void InitDrawCardProbability()
    {
        List<float> probabilityList = SoDataManager.instance.GetDrawHeroCardProbability();

        for(int i = 0; i < probabilityText.Length; i++)
        {
            probabilityText[i].text = string.Format("{0:D0}%", (int)(probabilityList[i] * 100f));
        }
    }

    private void Upgrade()
    {
        if (GameManager.instance.IsEnoughGold(cost))
        {
            InGameUI.instance.ClickCardRateUpgradeButton();
        }
    }
}

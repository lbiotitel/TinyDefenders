using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum RANK
{
    BRONZE,
    SILVER,
    GOLD,
    PLATINUM,
    DIAMOND
}

[System.Serializable]
public class RankNode
{
    public Sprite rankSprite;
    public RANK rank;
}

public class RankData
{
    public string userName;
    public string userTier;
    public double userRankPoint;
    public int userRank;
}

public class RankingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankingPointText;
    [SerializeField] private TextMeshProUGUI rankImageText;
    [SerializeField] private Image rankImage;
    [SerializeField] private RankNode[] rankSprite;

    [SerializeField] private GameObject rankingUINodePrefab;
    [SerializeField] private Transform rankingUINodeParent;

    private List<RankData> rankDataList;
    private RankData userRankData;
    private List<GameObject> rankUINodeList = new List<GameObject>();

    private void OnEnable()
    {
        GetUserLeaderBoard();
        GetLeaderBoard();
    }

    private void OnDisable()
    {
        ClearData();
    }

    private void ClearData()
    {
        rankDataList.Clear();
        userRankData = null;

        if (rankUINodeList.Count > 0)
            rankUINodeList.ForEach(g => Destroy(g.gameObject));
        rankUINodeList.Clear();
    }

    private async void GetUserLeaderBoard()
    {
        userRankData = await CloudManager.instance.GetUserLeaderBoard();
        ShowUserRankData();
    }

    private async void GetLeaderBoard()
    {
        rankDataList = await CloudManager.instance.GetLeaderBoard();
        ShowLeaderBoard();
    }

    private void ShowUserRankData()
    {
        rankingPointText.text = userRankData.userRankPoint.ToString() + " Rp";
        rankImage.sprite = GetTierImage(userRankData.userTier);
    }

    private Sprite GetTierImage(string tier)
    {
        switch (tier)
        {
            case "Bronze":
                return rankSprite[0].rankSprite;

            case "Silver":
                return rankSprite[1].rankSprite;
            case "Gold":
                return rankSprite[2].rankSprite;
            case "Platinum":
                return rankSprite[3].rankSprite;
            case "Diamond":
                return rankSprite[4].rankSprite;
        }

        return null;
    }


    private void ShowLeaderBoard()
    {

        for (int i = 0; i < rankDataList.Count; i++)
        {
            GameObject go = Instantiate(rankingUINodePrefab, rankingUINodeParent);
            RankUINode rankingUINode = go.GetComponent<RankUINode>();

            rankingUINode.InitRankUINode
            (
                rankDataList[i].userRank,
                rankDataList[i].userName,
                rankDataList[i].userRankPoint,
                GetTierImage(rankDataList[i].userTier)
             );

            rankUINodeList.Add(go);
        }
    }
}

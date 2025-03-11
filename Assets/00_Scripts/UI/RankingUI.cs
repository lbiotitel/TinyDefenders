using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum RANK { 
    BRONZE,
    SILVER,
    GOLD,
    PLATINUM,
    DIAMOND
}

[System.Serializable]
public class RankNode {
    public Sprite rankSprite;
    public int rankScoreUpperLine;
    public int rankScoreLowerLine;
    public RANK rank;
}

public class RankingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankingPointText;
    [SerializeField] private TextMeshProUGUI rankImageText;
    [SerializeField] private Image rankImage;
    [SerializeField] private RankNode[] rankSprite;

    private void OnEnable()
    {
        
    }

    private void ShowRankImage(int score)
    {
        for (int i = 0; i < rankSprite.Length; i++)
        {
            if (rankSprite[i].rankScoreLowerLine <= score && rankSprite[i].rankScoreUpperLine >= score)
            {
                rankImage.sprite = rankSprite[i].rankSprite;
                rankImageText.text = rankSprite[i].rank.ToString();
                break;
            }
        }
    }

    private void LoadUserRankingPoint()
    { 
        
    }

    private void LoadRank()
    { 
        
    }
}

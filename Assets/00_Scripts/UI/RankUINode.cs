using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankUINode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userRankText;
    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private TextMeshProUGUI userRankingPointText;
    [SerializeField] private Image userTierImage;


    public void InitRankUINode(int userRank,string userName,double userRp,Sprite tierImage)
    { 
        userRankText.text = userRank.ToString();   
        userNameText.text = userName;
        userRankingPointText.text = userRp.ToString(); 
        userTierImage.sprite = tierImage;
    }
}

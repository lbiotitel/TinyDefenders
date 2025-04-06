using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private TextMeshProUGUI userLevelText;
    [SerializeField] private TextMeshProUGUI userExpText;
    [SerializeField] private TextMeshProUGUI userRankPointText;
    [SerializeField] private TextMeshProUGUI userPlayStatText;
    [SerializeField] private Slider userExpSlider;
    [SerializeField] private Image userProfileImage;
    [SerializeField] private Image userRankImage;

    [SerializeField] private Button resetButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private RankNode[] rankSprite;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        exitButton.onClick.AddListener(HideProfile);
    }

    private void OnEnable()
    {
        InitUserProfile();
    }


    private async void InitUserProfile()
    {
        RankData ranKData = await CloudManager.instance.GetUserLeaderBoard();
        UserData userData = CloudManager.instance.GetUserData();

        userNameText.text = userData.userName;
        userLevelText.text = string.Format("LV.{0}",userData.userLevel);

        int requireExp = SoDataManager.instance.GetUserRequireExp(userData.userLevel);
        userExpText.text = string.Format("Exp : {0}/{1}", userData.userExp, requireExp);
        userExpSlider.maxValue = requireExp;
        userExpSlider.value = userData.userExp;

        userPlayStatText.text = string.Format("√— {0}∆« «√∑π¿Ã",userData.totalPlayCount);

        userRankImage.sprite = GetTierImage(ranKData.userTier);
        userRankPointText.text = string.Format("Rp Score : {0}", ranKData.userRankPoint);
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


    private void ResetProfile()
    {

    }


    public void ShowProfile()
    {
        gameObject.SetActive(true);
    }


    private void HideProfile()
    {
        animator.SetTrigger("HIDE");
    }
}

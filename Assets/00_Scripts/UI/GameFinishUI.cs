using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFinishUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI friendNameText;
    [SerializeField] private TextMeshProUGUI bestWaveText;
    [SerializeField] private TextMeshProUGUI playerAttackContributionText;
    [SerializeField] private TextMeshProUGUI friendAttackContributionText;
    [SerializeField] private Slider playerAttackContributionSlider;
    [SerializeField] private Slider friendAttackContributionSlider;

    [SerializeField] private TextMeshProUGUI crystalRewardText;
    [SerializeField] private TextMeshProUGUI goldRewardText;
    [SerializeField] private TextMeshProUGUI rpRewardText;

    [SerializeField] private Button exitButton;

    [SerializeField] private string showGameResultSFX;
    private GameFinishStatGameObject gameFinishStatGameObject;

    private void Awake()
    {
        FindGameFinishStatGameObject();
    }

    private void Start()
    {
        exitButton.onClick.AddListener(Exit);
        SetGameFinishUI();
    }

    private void FindGameFinishStatGameObject()
    { 
        gameFinishStatGameObject = FindAnyObjectByType<GameFinishStatGameObject>();
    }

    public void SetGameFinishUI() 
    {
        SoundManager.instance.PlaySfxSound(showGameResultSFX,SFXTYPE.UISOUND);  

        playerNameText.text = gameFinishStatGameObject.playerName;
        friendNameText.text= gameFinishStatGameObject.friendName;
        bestWaveText.text = gameFinishStatGameObject.bestWave.ToString();
        GetReward(gameFinishStatGameObject.bestWave);
        SetAttackContribution(gameFinishStatGameObject.playerDamage, gameFinishStatGameObject.friendDamage);
    }

    private void SetAttackContribution(double playerDamage,double friendDamage)
    {
        double total = playerDamage + friendDamage;

        double playerPercent = (playerDamage / total) * 100f;
        double friendPrecent = (friendDamage / total) * 100f;

        playerAttackContributionText.text = string.Format("{0:F0}%", playerPercent);
        friendAttackContributionText.text = string.Format("{0:F0}%", friendPrecent);

        playerAttackContributionSlider.value = (float)(playerPercent);
        friendAttackContributionSlider.value = (float)(friendPrecent);
    }

    private void GetReward(int bestWave)
    {

        int crystalReward = Mathf.FloorToInt(Mathf.Pow(bestWave, 1.5f) * 10);
        int goldReward = Mathf.FloorToInt(Mathf.Pow(bestWave, 1.5f) * 100);
        int rpReward = Mathf.FloorToInt(Mathf.Pow(bestWave, 1.5f) * 5);

        GetUserReward(crystalReward, goldReward, rpReward);

        crystalRewardText.text = string.Format("+{0}", crystalReward);
        goldRewardText.text = string.Format("+{0}", goldReward);
        rpRewardText.text = string.Format("+{0}", rpReward);
    }

    private void GetUserReward(int crystalReward, int goldReward, int rpReward)
    {
        UserData userData = CloudManager.instance.GetUserData();
        userData.gold += goldReward;
        userData.crystal += crystalReward;
        userData.rankingPoint += rpReward;
        CloudManager.instance.SaveUserData();
    }

    private void Exit()
    {
        Time.timeScale = 1f;
        Destroy(gameFinishStatGameObject.gameObject);
        SceneManager.LoadScene(SceneName.MainScene);
    }


    private void OnDestroy()
    {
        exitButton.onClick.RemoveAllListeners();
    }
}

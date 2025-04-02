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

    [SerializeField] private Button exitButton;
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
        playerNameText.text = gameFinishStatGameObject.playerName;
        friendNameText.text= gameFinishStatGameObject.friendName;
        bestWaveText.text = gameFinishStatGameObject.bestWave.ToString();
        SetAttackContribution(gameFinishStatGameObject.playerDamage, gameFinishStatGameObject.friendDamage);
        Time.timeScale = 0f;
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

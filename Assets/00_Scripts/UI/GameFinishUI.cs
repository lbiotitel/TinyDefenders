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


    private void Start()
    {
        exitButton.onClick.AddListener(Exit);
    }

    public void SetGameFinishUI(string playerName, string friendName, double playerDamage, double friendDamage, int bestWave) 
    { 
        playerNameText.text = playerName;
        friendNameText.text= friendName;
        bestWaveText.text = bestWave.ToString();
        SetAttackContribution(playerDamage, friendDamage);
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
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.Shutdown();
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneName.MainScene);
    }

    public void HideGameFinishUI()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        exitButton.onClick.RemoveAllListeners();
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI monsterCountText;
    [SerializeField] private TextMeshProUGUI summonGoldText;
    [SerializeField] private TextMeshProUGUI currentGoldText;

    [SerializeField] private Image monsterCountSliderImage;

    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private TextMeshProUGUI heroCountText;

    [SerializeField] private Animator assetAnimator;


    private void Start()
    {
        InitEvent();
    }

    private void InitEvent()
    {
        GameManager.instance.onGoldGet += GoldAnimation;
        GameManager.instance.onTimerUp += DrawWaveAndTime;
    }

    private void Update()
    {
        DrawMainUI();
    }

    private void DrawMainUI()
    {
        monsterCountText.text = 
            GameManager.instance.currentMonsterCount.ToString() + "/" + GameManager.instance.maxMonsterCount.ToString();
        monsterCountSliderImage.fillAmount = GameManager.instance.currentMonsterCount / 100f;
        summonGoldText.text = GameManager.instance.summonGold.ToString();
        currentGoldText.text = GameManager.instance.gold.ToString();
        summonGoldText.color = GameManager.instance.gold >= GameManager.instance.summonGold ? Color.white : Color.red;
        heroCountText.text = string.Format("{0:D2}/{1:D2}",GameManager.instance.heroCount,GameManager.instance.maxHeroCount);
    }

    private void DrawWaveAndTime()
    {
        timerText.text = GetTimeString();
        waveText.text = "WAVE\n" + GameManager.instance.wave.ToString();
    }

    private string GetTimeString()
    {
        int minutes = Mathf.FloorToInt(GameManager.instance.timer / 60);
        int seconds = Mathf.FloorToInt(GameManager.instance.timer % 60);

        return string.Format("{0:D2}:{1:D2}",minutes,seconds);
    }

    private void GoldAnimation()
    {
        assetAnimator.SetTrigger("GOLDGET");
    }
}

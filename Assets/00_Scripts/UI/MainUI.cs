using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public static MainUI instance = null;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    [SerializeField] private TextMeshProUGUI monsterCountText;
    [SerializeField] private TextMeshProUGUI summonGoldText;
    [SerializeField] private TextMeshProUGUI currentGoldText;

    [SerializeField] private Image monsterCountSliderImage;

    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI monsterNameText;

    [SerializeField] private TextMeshProUGUI heroCountText;

    [SerializeField] private Transform alarmBar;
    [SerializeField] private TextMeshProUGUI alarmTextPrefab;
    private List<TextMeshProUGUI> alarmTextList = new List<TextMeshProUGUI>();

    [SerializeField] private Animator assetAnimator;
    [SerializeField] private Animator profileAnimator;

    [Header("Summon Button")]
    [SerializeField] private Button summonButton;
    [SerializeField] private GameObject summonEffect;
    [SerializeField] private float effectSpeed;


    [Header("Upgarde UI")]
    [SerializeField] private TextMeshProUGUI[] upgradeLevelText;
    [SerializeField] private TextMeshProUGUI[] upgradeCostText;

    [Header("Wave UI")]
    [SerializeField] private GameObject waveUI;
    [SerializeField] private TextMeshProUGUI waveUIText;
    [SerializeField] private TextMeshProUGUI bossNameText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverUI;

    private void Start()
    {
        InitEvent();
        if (profileAnimator.gameObject.activeSelf == false)
            profileAnimator.gameObject.SetActive(true);
    }

    private void InitEvent()
    {
        GameManager.instance.onGoldGet += GoldAnimation;
        GameManager.instance.onTimerUp += DrawWaveAndTime;
        GameManager.instance.onGameOver += ShowGameOverUI;
        summonButton.onClick.AddListener(() => ClickSummon());
    }

    public void ShowWaveUI()
    {
        waveUI.gameObject.SetActive(true);
        waveUIText.text = string.Format("Wave {0}", GameManager.instance.currentWave.ToString());
    }

    public void ShowBossWaveUI(string bossName)
    { 
        waveUI.gameObject.SetActive(true);
        waveUI.GetComponent<Animator>().SetTrigger("BOSS");
        waveUIText.text = string.Format("Wave {0}", GameManager.instance.currentWave.ToString());
        bossNameText.text = bossName;
    }

    private void ClickSummon()
    {
        if (GameManager.instance.gold < GameManager.instance.summonGold) return;
        if (GameManager.instance.currentHeroCount >= GameManager.instance.maxHeroCount) return;

        GameManager.instance.gold -= GameManager.instance.summonGold;
        GameManager.instance.summonGold += GameManager.instance.summonGoldInc;

        StartCoroutine(SummonCoroutine());
    }

    public void ShowGameOverUI()
    {
        gameOverUI.SetActive(true);
    }

    public void ClickUpgrade(int value)
    {
        if (GameManager.instance.gold < 30 + GameManager.instance.upgarde[value])
            return;

        GameManager.instance.gold -= GameManager.instance.upgarde[value];
        GameManager.instance.upgarde[value]++;
    }

    private IEnumerator SummonCoroutine()
    {
        HeroScriptable heroData = Spawner.instance.LoadData("Normal");

        Vector3 targetPos = Spawner.instance.GetHolderPosition(heroData);

        Vector3 buttonWorldPosition = Camera.main.ScreenToWorldPoint(summonButton.transform.position);
        GameObject effect = Instantiate(summonEffect);

        effect.transform.position = buttonWorldPosition;

        Vector3 startPoint = buttonWorldPosition;
        Vector3 endPoint = targetPos;

        Vector3 randomPoint = MathUtils.GenerateRandomPoint(startPoint, endPoint);

        float elapsedTime = 0.0f;
        while (elapsedTime < effectSpeed)
        {
            float t = elapsedTime / effectSpeed;

            Vector3 curvePoint = MathUtils.GenerateBezierPoint(t, startPoint,
                randomPoint, endPoint);

            effect.transform.position = new Vector3(curvePoint.x, curvePoint.y, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(effect);
        Spawner.instance.Summon("Normal", heroData);
    }



    public void StartGame()
    {
        profileAnimator.gameObject.SetActive(false);
        GameManager.instance.StartGame();
    }

    private void Update()
    {
        DrawMainUI();
    }

    public void ShowAlarmText(string text)
    {
        if (alarmTextList.Count > 7)
        {
            Destroy(alarmTextList[0]);
            alarmTextList.RemoveAt(0);
        }
        TextMeshProUGUI alarmObj = Instantiate(alarmTextPrefab);
        alarmObj.text = text;
        alarmObj.transform.SetParent(alarmBar);
        alarmObj.transform.SetAsFirstSibling();
        alarmTextList.Add(alarmObj);
        Destroy(alarmObj, 1.5f);
    }

    private void DrawMainUI()
    {
        monsterCountText.text =
            GameManager.instance.currentMonsterCount.ToString() + "/" + GameManager.instance.maxMonsterCount.ToString();
        monsterCountSliderImage.fillAmount = GameManager.instance.currentMonsterCount / 100f;
        summonGoldText.text = GameManager.instance.summonGold.ToString();
        currentGoldText.text = GameManager.instance.gold.ToString();
        summonGoldText.color = GameManager.instance.gold >= GameManager.instance.summonGold ? Color.white : Color.red;
        heroCountText.text = string.Format("{0:D2}/{1:D2}", GameManager.instance.currentHeroCount, GameManager.instance.maxHeroCount);

        for (int i = 0; i < upgradeLevelText.Length; i++)
        {
            upgradeLevelText[i].text = "Lv." + (GameManager.instance.upgarde[i] + 1).ToString();
            upgradeCostText[i].text = (30 + GameManager.instance.upgarde[i]).ToString();
        }
    }

    private void DrawWaveAndTime(string monsterName)
    {
        timerText.text = GetTimeString();
        waveText.text = "WAVE " + GameManager.instance.currentWave.ToString();
        monsterNameText.text = monsterName;
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

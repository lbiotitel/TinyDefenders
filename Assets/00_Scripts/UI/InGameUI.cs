using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : NetworkBehaviour
{
    public static InGameUI instance = null;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    [SerializeField] private TextMeshProUGUI monsterCountText;
    [SerializeField] private TextMeshProUGUI currentGoldText;

    [SerializeField] private Image monsterCountSliderImage;

    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI monsterNameText;

    [SerializeField] private Transform alarmBar;
    [SerializeField] private TextMeshProUGUI alarmTextPrefab;
    private List<TextMeshProUGUI> alarmTextList = new List<TextMeshProUGUI>();

    [Header("Game Start UI")]
    [SerializeField] private Animator profileAnimator;
    [SerializeField] private TextMeshProUGUI playerProfileNameText;
    [SerializeField] private TextMeshProUGUI friendProfileNameText;
    [SerializeField] private TextMeshProUGUI playerProfileAnimNameText;
    [SerializeField] private TextMeshProUGUI friendProfileAnimNameText;
    public bool profileAnimationReady = false;

    [Header("Upgarde UI")]
    [SerializeField] private TextMeshProUGUI[] upgradeLevelText;
    [SerializeField] private TextMeshProUGUI[] upgradeCostText;

    [Header("Wave UI")]
    [SerializeField] private GameObject waveUI;
    [SerializeField] private TextMeshProUGUI waveUIText;
    [SerializeField] private TextMeshProUGUI bossNameText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI bestWaveText;
    [SerializeField] private TextMeshProUGUI playerGameOverNameText;
    [SerializeField] private TextMeshProUGUI friendGameOverNameText;
    [SerializeField] private TextMeshProUGUI playerDpsText;
    [SerializeField] private TextMeshProUGUI friendDpsText;

    [Header("Alarm UI")]
    [SerializeField] private AlarmUI alarmUI;

    [Header("Hero Info")]
    [SerializeField] private HeroInfoUI heroInfoUI;

    [Header("Card Arrow")]
    [SerializeField] private CardArrowGuide arrowGuide;

    [Header("Card Range UI")]
    [SerializeField] private GameObject cardRangeUI;


    private void Start()
    {
        InitEvent();
        if (profileAnimator.gameObject.activeSelf == false)
            profileAnimator.gameObject.SetActive(true);
        heroInfoUI.HideHeroInfoUI();
    }


    public void InitAllUserName(string hostName, string clientName)
    { 
        playerProfileNameText.text = hostName;
        playerGameOverNameText.text = hostName;
        playerProfileAnimNameText.text = hostName;
        friendProfileNameText.text = clientName;
        friendGameOverNameText.text = clientName;
        friendProfileAnimNameText.text = clientName;
    }

    private void InitEvent()
    {
        GameManager.instance.onTimerUp += DrawWaveAndTime;
        GameManager.instance.onGameOver += ShowGameOverUI;
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

  
    public void ShowGameOverUI()
    {
        gameOverUI.SetActive(true);
        StartCoroutine(ShowGameOverDelay());
    }

    private IEnumerator ShowGameOverDelay()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        bestWaveText.text = GameManager.instance.currentWave.ToString();
        playerDpsText.text = GameManager.instance.playerDps.ToString();
        friendDpsText.text = GameManager.instance.friendDps.ToString();
    }

    public void ClickUpgrade(int value)
    {
        if (GameManager.instance.gold < 30 + GameManager.instance.upgarde[value])
            return;

        GameManager.instance.gold -= GameManager.instance.upgarde[value];
        GameManager.instance.upgarde[value]++;
    }


    [ServerRpc(RequireOwnership = false)]
    public void ShowAlarmServerRpc(string alarmText, float duration, string strAlarmType)
    {
        ShowAlarmClientRpc(alarmText,duration,strAlarmType);
    }

    [ClientRpc]
    public void ShowAlarmClientRpc(string alarmText, float duration,string strAlarmType)
    {
        AlarmType alarmType = (AlarmType)System.Enum.Parse(typeof(AlarmType), strAlarmType);
        alarmUI.AddAlarm(alarmText,duration,alarmType);
    }

    public void HideProfileAnimation()
    {
        profileAnimator.SetTrigger("HIDE");
    }

    private void Update()
    {
        DrawMainUI();
    }

    public void ShowHeroInfoUI(Hero hero)
    { 
        heroInfoUI.ShowHeroInfoUI(hero);
    }

    public void HideHeroInfoUI()
    {
        heroInfoUI.HideHeroInfoUI();
    }

    public void ShowArrowGuide(RectTransform startPos)
    {
        arrowGuide.ShowArrow(startPos);
    }

    public void HideArrowGuide()
    {
        arrowGuide.HideArrow();
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
        currentGoldText.text = GameManager.instance.gold.ToString();

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


    #region About CardRange UI
    public void ShowCardRangeUI(Vector3 targetPos ,float range)
    {
        cardRangeUI.transform.position = targetPos;
        cardRangeUI.transform.localScale = Vector3.one * range;
        cardRangeUI.gameObject.SetActive(true);
    }

    public void HideCardRangeUI() 
    { 
        cardRangeUI.gameObject.SetActive(false);
    }

    #endregion
}

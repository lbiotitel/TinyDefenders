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

    [Header("Game Start UI")]
    [SerializeField] private Animator profileAnimator;
    [SerializeField] private TextMeshProUGUI playerProfileNameText;
    [SerializeField] private TextMeshProUGUI friendProfileNameText;
    [SerializeField] private TextMeshProUGUI playerProfileAnimNameText;
    [SerializeField] private TextMeshProUGUI friendProfileAnimNameText;
    [SerializeField] private TextMeshProUGUI countDownText;
    public bool profileAnimationReady = false;

    [Header("Upgarde UI")]
    [SerializeField] private UpgradeUI upgradeUI;

    [Header("Hero Combine UI")]
    [SerializeField] private CombineUI combineUI;

    [Header("Select Relic UI")]
    [SerializeField] private SelectRelicCardUI selectRelicCardUI;

    [Header("Quest UI")]
    [SerializeField] private InGameQuestUI inGameQuestUI;

    [Header("Wave UI")]
    [SerializeField] private GameObject waveUI;
    [SerializeField] private TextMeshProUGUI waveUIText;
    [SerializeField] private TextMeshProUGUI bossNameText;

    [Header("Game Over UI")]
    [SerializeField] GameObject gameOverUI;

    [Header("NetWorkAlarm UI")]
    [SerializeField] private NetworkAlarmUI networkAlramUI;

    [Header("Hero Info")]
    [SerializeField] private HeroInfoUI heroInfoUI;

    [Header("Card Range UI")]
    [SerializeField] private GameObject cardRangeUI;

    [Header("Option UI")]
    [SerializeField] private InGameOptionUI optionUI;

    [Header("Gold Text Prefab")]
    [SerializeField] private Transform getGoldTextTransform;
    [SerializeField] private GameObject getGoldPrefab;


  
    private void Start()
    {
        InitEvent();
        if (profileAnimator.gameObject.activeSelf == false)
            profileAnimator.gameObject.SetActive(true);
        heroInfoUI.HideHeroInfoUI();
        RedrawUpgradeUI();
    }


    public void InitAllUserName(string hostName, string clientName)
    {
        playerProfileNameText.text = hostName;
        playerProfileAnimNameText.text = hostName;
        friendProfileNameText.text = clientName;
        friendProfileAnimNameText.text = clientName;
    }

    public void ShowCountDownText(int count)
    {
        countDownText.transform.parent.gameObject.SetActive(true);
        countDownText.text = count.ToString();
    }

    public void HideCountDownText()
    {
        countDownText.transform.parent.GetComponent<Animator>().SetTrigger("HIDE");
    }


    private void InitEvent()
    {
        EventManager.instance.onTimerTickAction+= DrawWaveAndTime;
        EventManager.instance.onBossWaveClear += ShowSelectRelicUI;
    }

    public void ShowWaveUI()
    {
        waveUI.gameObject.SetActive(true);
        waveUIText.text = string.Format("Wave {0}", GameManager.instance.GetCurrentWave().ToString());
    }

    public void ShowBossWaveUI(string bossName)
    {
        waveUI.gameObject.SetActive(true);
        waveUI.GetComponent<Animator>().SetTrigger("BOSS");
        waveUIText.text = string.Format("Wave {0}", GameManager.instance.GetCurrentWave().ToString());
        bossNameText.text = bossName;
    }


    #region About GameOver UI
    public void ShowGameOverUI()
    {
        gameOverUI.SetActive(true);
    }


    #endregion



    [ServerRpc(RequireOwnership = false)]
    public void ShowAlarmServerRpc(string alarmText, float duration, string strAlarmType)
    {
        ShowAlarmClientRpc(alarmText, duration, strAlarmType);
    }

    [ClientRpc]
    public void ShowAlarmClientRpc(string alarmText, float duration ,string strAlarmType)
    {
        AlarmType alarmType = (AlarmType)System.Enum.Parse(typeof(AlarmType), strAlarmType);
        networkAlramUI.AddAlarm(alarmText, duration, alarmType);
    }

    public void ShowAlarm(string alarmText, float duration, AlarmType alarmType)
    {
        networkAlramUI.AddAlarm(alarmText,duration, alarmType);
    }

    public void HideProfileAnimation()
    {
        profileAnimator.SetTrigger("HIDE");
    }

    private void Update()
    {
        DrawMainUI();
        CheckOptionKey();
        CheckHeroRecipeKey();
        CheckHeroUpgradeKey();
    }

    private void CheckOptionKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!optionUI.gameObject.activeSelf) optionUI.ShowOptionUI();
            else optionUI.HideOptionUI();
        }
    }

    private void CheckHeroUpgradeKey()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!upgradeUI.gameObject.activeSelf) upgradeUI.ShowUpgradeUI();
            else upgradeUI.HideUpgradeUI();
        }
    }

    private void CheckHeroRecipeKey()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!combineUI.gameObject.activeSelf) combineUI.ShowHeroCombineUI();
            else combineUI.HideHeroCombineUI();
        }
    }

    public void ShowHeroInfoUI(Hero hero)
    {
        heroInfoUI.ShowHeroInfoUI(hero);
    }

    public void HideHeroInfoUI()
    {
        heroInfoUI.HideHeroInfoUI();
    }

    private void DrawMainUI()
    {
        monsterCountText.text =
            GameManager.instance.currentMonsterCount.ToString() + "/" + GameManager.instance.maxMonsterCount.ToString();
        monsterCountSliderImage.fillAmount = GameManager.instance.currentMonsterCount / 100f;
        currentGoldText.text = string.Format("{0}G", GameManager.instance.gold);

    }

    private void DrawWaveAndTime(float time)
    {
        timerText.text = GetTimeString(time);
        waveText.text = "WAVE " + GameManager.instance.GetCurrentWave().ToString();
    }

    private string GetTimeString(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        return string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }


    public void ShowGetGoldText(int gold)
    {
        GameObject go = PoolingManager.instance.GetObject(getGoldPrefab, getGoldTextTransform.position);
        go.GetComponent<TextMeshProUGUI>().text = string.Format("+{0}G", gold);
        go.transform.SetParent(getGoldTextTransform);
    }


    #region About CardRange UI
    public void ShowCardRangeUI(Vector2 targetPos, float range)
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

    #region About Upgrade UI

    public void RedrawUpgradeUI()
    {
        upgradeUI.RedrawUpgradeUINodes();
    }

    public void ClickUpgradeButton(HeroTribe heroTribe)
    {
        upgradeUI.ClickHeroUpgradeButton(heroTribe);
    }

    public void ClickCardRateUpgradeButton()
    {
        upgradeUI.ClickDrawCardRateUpgrade();
    }

    #endregion


    #region Select Relic Card UI
    private void ShowSelectRelicUI()
    { 
        selectRelicCardUI.ShowSelectRelicCardUI();
    }

    private void HideSelectRelicUI()
    {
        selectRelicCardUI.HideSelectRelicUI();
    }
    #endregion
}

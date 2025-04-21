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

    [Header("Game Stat")]
    [SerializeField] private TMP_Text currentGoldText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text currentHeroCountText;
    [SerializeField] private TMP_Text maxHeroCountText;
    [SerializeField] private InGameLifeContainer lifeContainer;


    [Header("Player Stat UI")]
    [SerializeField] private TMP_Text ingameLevelText;
    [SerializeField] private TMP_Text ingameExpText;
    [SerializeField] private Slider ingameExpSlider;


    [Header("Reroll Button")]
    [SerializeField] private Button rerollButton;

    [Header("Exp Button")]
    [SerializeField] private Button expButton;

    [Header("Game Start UI")]
    [SerializeField] private Animator profileAnimator;
    [SerializeField] private TMP_Text playerProfileNameText;
    [SerializeField] private TMP_Text playerLevelText;
    [SerializeField] private TMP_Text clientProfileNameText;
    [SerializeField] private TMP_Text clientLevelText;

    [SerializeField] private TMP_Text playerProfileAnimNameText;
    [SerializeField] private TMP_Text clientProfileAnimNameText;
    [SerializeField] private TMP_Text countDownText;
    public bool profileAnimationReady = false;

    [Header("Upgarde UI")]
    [SerializeField] private UpgradeUI upgradeUI;

    [Header("Select Relic UI")]
    [SerializeField] private SelectRelicCardUI selectRelicCardUI;

    [Header("Quest UI")]
    [SerializeField] private InGameQuestUI inGameQuestUI;

    [Header("Wave UI")]
    [SerializeField] private GameObject waveUI;
    [SerializeField] private TMP_Text waveUIText;
    [SerializeField] private TMP_Text bossNameText;

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

        rerollButton.onClick.AddListener(() => CardManager.instance.RerollCard(false));
        expButton.onClick.AddListener(() => PlayerStatManager.instance.AddIngameExp());

        EventManager.instance.onLevelChagendAction += (int level) => { ingameLevelText.text = string.Format("LV.{0}", level); };
        EventManager.instance.onExpLevelChangedAction += (int exp,int requireExp) => 
        {
            ingameExpText.text = string.Format("{0}/{1}",exp, requireExp);
            ingameExpSlider.value = exp;
            ingameExpSlider.maxValue = requireExp;
        };

        EventManager.instance.onHeroSummonAction += ShowCurrentHeroCount;
        EventManager.instance.onMaxHeroCountChangedAction += ShowMaxHeroCount;
    }


    public void InitAllUserName(string hostName, string clientName)
    {
        playerProfileNameText.text = hostName;
        playerProfileAnimNameText.text = hostName;
        clientProfileNameText.text = clientName;
        clientProfileAnimNameText.text = clientName;
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
        EventManager.instance.onTimerTickAction += DrawWaveAndTime;
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

    [ClientRpc]
    public void RedrawLifeContainerClientRpc(int life)
    { 
        lifeContainer.ShowLifeNode(life);
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

    private void ShowCurrentHeroCount(Hero hero)
    {
        int heroCount = SpawnManager.instance.GetHeroCount(NetUtils.GetPlayerNetworkType());
        currentHeroCountText.text = string.Format("{0:D2}", heroCount);
    }

    private void ShowMaxHeroCount(int maxHeroCount)
    {
        maxHeroCountText.text = string.Format("/{0:D2}", maxHeroCount);
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
        currentGoldText.text = string.Format("{0}G", GameManager.instance.gold);
    }

    private void DrawWaveAndTime(float time, bool prepare = false)
    {
        timerText.text = GetTimeString(time);
        if (!prepare)
        {
            waveText.text = "WAVE " + GameManager.instance.GetCurrentWave().ToString();
        }
        else {
            waveText.text = "¡ÿ∫Ò";
        }
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

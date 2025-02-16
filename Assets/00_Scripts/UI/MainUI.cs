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

    [SerializeField] private TextMeshProUGUI heroCountText;

    [SerializeField] private Transform alarmBar;
    [SerializeField] private TextMeshProUGUI alarmTextPrefab;
    private List<TextMeshProUGUI> alarmTextList = new List<TextMeshProUGUI>();

    [SerializeField] private Animator assetAnimator;
    [SerializeField] private Animator profileAnimator;

    [SerializeField] private Button summonButton;
    [SerializeField] private GameObject summonEffect;
    [SerializeField] private float effectSpeed;


    private void Start()
    {
        InitEvent();
        if(profileAnimator.gameObject.activeSelf == false)
            profileAnimator.gameObject.SetActive(true);
    }

    private void InitEvent()
    {
        GameManager.instance.onGoldGet += GoldAnimation;
        GameManager.instance.onTimerUp += DrawWaveAndTime;
        summonButton.onClick.AddListener(() => ClickSummon());
    }

    private void ClickSummon()
    {
        StartCoroutine(SummonCoroutine());
    }

    private IEnumerator SummonCoroutine()
    {
        HeroScriptable heroData = Spawner.instance.LoadData("Normal");

        Vector3 targetPos =  Spawner.instance.GetHolderPosition(heroData);

        Vector3 buttonWorldPosition = Camera.main.ScreenToWorldPoint(summonButton.transform.position);
        GameObject effect = Instantiate(summonEffect);

        effect.transform.position = buttonWorldPosition;

        Vector3 startPoint = buttonWorldPosition;
        Vector3 endPoint = targetPos;

        Vector3 randomPoint = GenerateRandomPoint(startPoint,endPoint);

        float elapsedTime = 0.0f;
        while (elapsedTime < effectSpeed)
        {
            float t= elapsedTime / effectSpeed;

            Vector3 curvePoint = GenerateBezierPoint(t,startPoint,
                randomPoint,endPoint);

            effect.transform.position = new Vector3(curvePoint.x,curvePoint.y,0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        Destroy(effect);
        Spawner.instance.Summon("Normal", false, heroData);
    }

    private Vector3 GenerateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return Mathf.Pow(1-t,2) * p0 + 2 * (1-t) * t * p1 + (t*t) * p2;
    }

    private Vector3 GenerateRandomPoint(Vector3 start, Vector3 end)
    {
        Vector3 midPoint = (start + end) / 2f;
        float randomHeight = Random.Range(1.0f,3.0f);
        midPoint += Vector3.up * randomHeight;

        midPoint += new Vector3(Random.Range(-1.0f,1.0f), 0.0f);

        return midPoint;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        profileAnimator.gameObject.SetActive(false);
        Spawner.instance.StartMonsterSpawn();
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
        Destroy(alarmObj,1.5f);
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

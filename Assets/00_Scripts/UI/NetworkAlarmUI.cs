using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class NetworkAlarmUI : MonoBehaviour
{
    private class AlarmData
    {
        public string alarmText;
        public float alarmDuration;
        public AlarmType alarmType = AlarmType.NORMAL;

        public AlarmData(string alarmText, float alarmDuration, AlarmType alarmType)
        {
            this.alarmText = alarmText;
            this.alarmDuration = alarmDuration;
            this.alarmType = alarmType;
        }
    }

    [System.Serializable]
    public class AlarmSprite {
        public AlarmType alarmType;
        public Sprite alarmImage;
    }

    [SerializeField] private RectTransform horizontalRectTransform;
    [SerializeField] private TextMeshProUGUI alarmText;
    [SerializeField] private Image alarmImage;
    [SerializeField] private List<AlarmSprite> alarmSprites;

    private Queue<AlarmData> alarmDatas = new Queue<AlarmData>();
    private Animator animator;
    bool canShowAlarm = true;

    public void FinishAlarm()
    {
        canShowAlarm = true;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        alarmImage.preserveAspect = true;
        HideAlarm(0.1f);
    }

    private void Update()
    {
        if (canShowAlarm)
        {
            ShowAlarm();
        }
    }

    public void AddAlarm(string alarmText, float duration,AlarmType alarmType = AlarmType.NORMAL)
    {
        alarmDatas.Enqueue(new AlarmData(alarmText, duration, alarmType));
    }

    private void ShowAlarm()
    {
        if (alarmDatas.Count <= 0) return;
        
        AlarmData alarmData = alarmDatas.Dequeue();

        animator.SetTrigger("SHOW");

        alarmText.text = alarmData.alarmText;
        canShowAlarm = false;
        AlarmTypeLogic(alarmData.alarmType);
        LayoutRebuilder.ForceRebuildLayoutImmediate(horizontalRectTransform);

        StartCoroutine(HideAlarm(alarmData.alarmDuration));
    }

    private void AlarmTypeLogic(AlarmType alarmType)
    {
        alarmImage.gameObject.SetActive(false);
        switch (alarmType)
        {
            case AlarmType.NORMAL:
                
                break;
            case AlarmType.GOOD:
                //SoundManager.instance.PlaySfxSound("GoodAlarmSFX");
                break;
            case AlarmType.BAD:

                break;
            case AlarmType.WARNING:
                alarmImage.sprite = alarmSprites[(int)AlarmType.WARNING].alarmImage;
                alarmImage.gameObject.SetActive(true);
                break;
            case AlarmType.QUEST:
                alarmImage.sprite = alarmSprites[(int)AlarmType.QUEST].alarmImage;
                alarmImage.gameObject.SetActive(true);
                break;
        }
    }

    private IEnumerator HideAlarm(float duration)
    {
        yield return new WaitForSeconds(duration);
        animator.SetTrigger("HIDE");
    }

}

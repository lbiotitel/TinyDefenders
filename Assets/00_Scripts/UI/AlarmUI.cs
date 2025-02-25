using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class AlarmUI : MonoBehaviour
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

    [SerializeField] private TextMeshProUGUI alarmText;
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
    }

    private void Update()
    {
        if (canShowAlarm)
        {
            ShowAlarm();
            Debug.Log("¼î ¾Ë¶÷");
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
        PlayAlarmSound(alarmData.alarmType);
        StartCoroutine(HideAlarm(alarmData.alarmDuration));
    }

    private void PlayAlarmSound(AlarmType alarmType)
    {
        switch (alarmType)
        {
            case AlarmType.NORMAL:

                break;
            case AlarmType.GOOD:
                SoundManager.instance.PlaySfxSound("GoodAlarmSFX");
                break;
            case AlarmType.BAD:
                break;
        }
    }

    private IEnumerator HideAlarm(float duration)
    {
        yield return new WaitForSeconds(duration);
        animator.SetTrigger("HIDE");
    }

}

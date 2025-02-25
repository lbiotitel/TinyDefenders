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

        public AlarmData(string alarmText, float alarmDuration)
        { 
            this.alarmText = alarmText;
            this.alarmDuration = alarmDuration;
        }
    }

    [SerializeField] private TextMeshProUGUI alarmText;
    private Queue<AlarmData> alarmDatas = new Queue<AlarmData>();
    private Animator animator;
    bool canShowAlarm = true;

    public void FinishAlarm()
    {
        canShowAlarm = true;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (canShowAlarm)
        {
            canShowAlarm = false;
            ShowAlarm();
        }
    }

    public void AddAlarm(string alarmText, float duration)
    {
        alarmDatas.Enqueue(new AlarmData(alarmText, duration));
    }

    private void ShowAlarm()
    {
        if (alarmDatas.Count <= 0) return;
        animator.SetTrigger("SHOW");
        AlarmData alarmData = alarmDatas.Dequeue();
        alarmText.text = alarmData.alarmText;
        StartCoroutine(HideAlarm(alarmData.alarmDuration));
        gameObject.SetActive(true);
    }

    private IEnumerator HideAlarm(float duration)
    {
        yield return new WaitForSeconds(duration);
        animator.SetTrigger("HIDE");
    }

}

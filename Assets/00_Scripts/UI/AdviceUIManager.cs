using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AdviceUIManager : MonoBehaviour
{
    public static AdviceUIManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    [SerializeField] private TMP_Text adviceText;
    [SerializeField] private float adviceTextCoolTime;
    [SerializeField] private List<string> adviceList = new List<string>();
    private bool canShowAdvice = true;
    private Animator adviceAnimator;

    private void Start()
    {
        HideAdviceText();
        adviceAnimator = adviceText.gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        CheckAdviceInList();
    }

    private void CheckAdviceInList()
    {
        if (canShowAdvice)
        {
            if (adviceList.Count > 0)
            {
                ShowAdvice();
                canShowAdvice = false;
            }
        }
    }

    public void AddAdvice(string advice)
    {
        adviceList.Add(advice);
    }

    private void ShowAdvice()
    {
        string node = adviceList[0];
        adviceText.text = node;
        adviceList.Remove(node);
        adviceText.gameObject.SetActive(true);

        StartCoroutine(AdviceTextCoolTimeCouroutine());
    }

    private IEnumerator AdviceTextCoolTimeCouroutine()
    { 
        yield return new WaitForSeconds(adviceTextCoolTime);

        HideAdviceText();
        canShowAdvice = true;
    }


    private void HideAdviceText()
    { 
        if(adviceText.gameObject.activeSelf)
            adviceAnimator.SetTrigger("HIDE");
    }

}

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

    [System.Serializable]
    public class AdviceQueueNode
    {
        public string advice;
        public float lifeTime;

        public AdviceQueueNode(string advice, float lifeTime)
        {
            this.advice = advice;
            this.lifeTime = lifeTime;
        }
    }

    [SerializeField] private TMP_Text adviceText;
    [SerializeField] private List<AdviceQueueNode> adviceList = new List<AdviceQueueNode>();
    private Animator adviceAnimator;

    private void Start()
    {
        HideAdviceText();
        adviceAnimator = adviceText.gameObject.GetComponent<Animator>();
    }


    public void AddAdvice(string advice, float lifeTime = 1f)
    {
        adviceList.Add(new AdviceQueueNode(advice,lifeTime));
        ShowAdvice();
    }

    private void ShowAdvice()
    {
        AdviceQueueNode node = adviceList[0];
        adviceList.Remove(node);
        adviceText.text = node.advice;
        adviceText.gameObject.SetActive(true);

        StartCoroutine(AdviceLifeCoroutine(node.lifeTime));
    }

    private IEnumerator AdviceLifeCoroutine(float lifeTime)
    { 
        yield return new WaitForSeconds(lifeTime);

        HideAdviceText();
        if (adviceList.Count > 0) ShowAdvice();
    }


    private void HideAdviceText()
    { 
        if(adviceText.gameObject.activeSelf)
            adviceAnimator.SetTrigger("HIDE");
    }

}

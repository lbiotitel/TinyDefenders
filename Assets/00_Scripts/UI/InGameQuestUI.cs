using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InGameQuestUI : MonoBehaviour
{
    [SerializeField] private InGameQuestUINode InGameQuestUINode;
    [SerializeField] private Transform contentTransform;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        InitInGameQuestUI();
    }

    private void InitInGameQuestUI()
    { 
        List<InGameQuestSO> questList = SoDataManager.instance.GetAllInGameQuest();

        foreach (var quest in questList)
        {
            InGameQuestUINode questNode = Instantiate(InGameQuestUINode, contentTransform);
            questNode.Init(quest);
        }

    }

    public void ShowInGameQuestUI()
    {
        gameObject.SetActive(true);
    }

    public void HideInGameQuestUI()
    {
        animator.SetTrigger("HIDE");
    }
}

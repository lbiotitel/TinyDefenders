using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillUI : MonoBehaviour
{
    [SerializeField] private PlayerSkillUINode playerSkillUINodePrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private TMP_Text skillPointText;
    [SerializeField] private Button savePlayerSkillButton;
    private int remainSkillPoint = 0;

    private void Start()
    {
        InitPlayerSkillUINode();
        savePlayerSkillButton.onClick.AddListener(SaveSkill);
    }


    private void SaveSkill()
    { 
        
    }


    private void InitPlayerSkillUINode()
    {
        List<PlayerSkillSO> playerSkillSOs = SoDataManager.instance.GetAllPlayerSkillData();

        foreach (var playerSkillSO in playerSkillSOs)
        {
            PlayerSkillUINode playerSkillUINode = Instantiate(playerSkillUINodePrefab, contentParent);
            playerSkillUINode.InitPlayerSkillUINode(playerSkillSO);
        }
    }

}

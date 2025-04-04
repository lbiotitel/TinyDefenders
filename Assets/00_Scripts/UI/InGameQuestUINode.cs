using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameQuestUINode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private GameObject questHeroImagePrefab;
    [SerializeField] private GameObject clearObject;
    private InGameQuestSO inGameQuestSO;

    public void Init(InGameQuestSO inGameQuestSO)
    {
        this.inGameQuestSO = inGameQuestSO;
        questNameText.text = inGameQuestSO.questName;

        foreach (var heroImage in this.inGameQuestSO.requireHeroes)
        { 
            GameObject go = Instantiate(questHeroImagePrefab, contentTransform);
            Image image = go.GetComponent<Image>();
            image.sprite = heroImage.heroMainImage;
        }

        EventManager.instance.onHeroSummonAction += CheckQuest;
        EventManager.instance.onHeroDespawnAction += CheckQuest;
    }

    private void CheckQuest(Hero hero)
    {
        List<Hero> heroList = SpawnManager.instance.GetHeroList(NetUtils.GetPlayerNetworkType());
        bool isClear = inGameQuestSO.CheckQuest(ref heroList);

        if (isClear)
        {
            clearObject.SetActive(true);

            GameManager.instance.GetGold(inGameQuestSO.questGold, NetUtils.GetPlayerNetworkType());

            InGameUI.instance.ShowAlarm(string.Format("{0} Clear",inGameQuestSO.questName),1f,AlarmType.QUEST);

            EventManager.instance.onHeroSummonAction -= CheckQuest;
            EventManager.instance.onHeroDespawnAction -= CheckQuest;

        }
    }

}

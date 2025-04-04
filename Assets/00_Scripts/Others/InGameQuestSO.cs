using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

[CreateAssetMenu(fileName = "InGameQuestSO", menuName = "Scriptable Objects/InGameQuestSO")]
public class InGameQuestSO : ScriptableObject
{
    public string questName;
    public int questGold;
    public HeroScriptable[] requireHeroes;
    public HeroRarity[] requireRarity;

    private bool isClear = false;
    private bool[] visited;

    private void OnValidate()
    {
        questName = name;
    }

    public bool CheckQuest(ref List<Hero> heroList)
    {
        if (isClear) return true;
        if (heroList.Count < requireHeroes.Length) return false;

        bool[] isExist = new bool[requireHeroes.Length];
        visited = new bool[heroList.Count];

        for (int i = 0; i < isExist.Length; i++) isExist[i] = false;

        CheckQuestRecur(0,ref heroList,ref isExist);

        for (int i = 0; i < isExist.Length; i++)
        {
            if (isExist[i] == false)
            {
                isClear = false;
                return false;
            }
        }

        isClear = true;
        return true;
    }

    private void CheckQuestRecur(int index, ref List<Hero> heroList,ref bool[] isExist)
    {
        if (index == requireHeroes.Length) return;


        for (int i = 0; i < heroList.Count; i++)
        {
            if (visited[i] == true) continue;
            if (heroList[i].heroName == requireHeroes[index].heroName)
            {
                visited[i] = true;
                isExist[index] = true;
                CheckQuestRecur(index + 1,ref heroList, ref isExist);
                visited[i] = false;
            }
        }
    }


    public string GetLocalizationQuetName()
    {
        return questName;
        //Utils.Localization_Text();
    }
}

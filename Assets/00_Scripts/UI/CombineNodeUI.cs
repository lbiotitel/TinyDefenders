using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombineNodeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private TextMeshProUGUI heroDesc;
    [SerializeField] private Transform horizontalLayout;
    [SerializeField] private Button combineButton;

    private HeroScriptable mainHero;
    private List<HeroScriptable> subHero;

    private bool canCombine = false;

    private void Start()
    {
        combineButton.onClick.AddListener(() => { Combine(); });
    }

    public void SetName(string name) { heroName.text = name; }

    public void SetHeroData(HeroScriptable mainHero, List<HeroScriptable> subHero)
    { 
        this.mainHero = mainHero;   
        this.subHero = subHero;
        CanCombineHero();
    }

    public void AddHeroUI(GameObject gameObject)
    {
        gameObject.transform.SetParent(horizontalLayout);
    }

    private void Combine()
    { 
        
    }

    private void CanCombineHero()
    {
        List<HeroScriptable> heroList = new List<HeroScriptable>(subHero);
        foreach (var heroHolder in Spawner.instance.heroHolders)
        {
            if(heroHolder.Value.heroList.Count > 0)
            {
                for (int i = 0; i < heroHolder.Value.heroList.Count; i++)
                {
                    for (int j = 0; j < heroList.Count; j++)
                    {
                        if (heroList[j].heroName == heroHolder.Value.heroList[i].heroName)
                        {
                            heroList.Remove(heroList[j]);
                        }
                    }
                }
            }
        }

        if (heroList.Count <= 0)
        {
            combineButton.gameObject.SetActive(true);
            canCombine = true;
        }
        else {
            canCombine = false;
        }
    }
}

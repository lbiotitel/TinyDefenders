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
    [SerializeField] private GameObject activeMarkObject;

    private HeroScriptable mainHero;
    private List<HeroScriptable> subHero;
    private List<HeroHolder> heroHolders = new List<HeroHolder>();

    private bool canCombine = false;

    private void Start()
    {
        combineButton.onClick.AddListener(() => { Combine(); });
        GameManager.instance.onSummonHero += CanCombineHero;
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
        if (canCombine)
        {
            for (int i = 0; i < heroHolders.Count; i++)
            {
                heroHolders[i].Sell(false);
            }

            Spawner.instance.Summon(mainHero.rarity.ToString(),mainHero);
        }
    }

    public void CanCombineHero()
    {
        List<HeroScriptable> heroList = new List<HeroScriptable>(subHero);
        foreach (var heroHolder in Spawner.instance.heroHolders)
        {
            if (heroHolder.Value.holderId.Contains(NetUtils.IsServer() ? "HOST" : "CLIENT"))
            {
                if (heroHolder.Value.ownHero != null)
                {
                    for (int j = 0; j < heroList.Count; j++)
                    {
                        if (heroList[j].heroName == heroHolder.Value.ownHero.heroName)
                        {
                            heroList.Remove(heroList[j]);
                            heroHolders.Add(heroHolder.Value);
                        }
                    }
                }
            }
        }

        if (heroList.Count <= 0)
        {
            activeMarkObject.gameObject.SetActive(false);
            canCombine = true;
        }
        else {
            activeMarkObject.gameObject.SetActive(true);
            canCombine = false;
            heroHolders.Clear();
        }
    }
}

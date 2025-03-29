using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombineNodeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private Image heroImage;
    [SerializeField] private Button combineButton;

    private HeroScriptable mainHero;
    private List<HeroScriptable> subHero;
    private List<HeroHolder> heroHolders = new List<HeroHolder>();

    private bool canCombine = false;

    private void Start()
    {
        combineButton.onClick.AddListener(() => { Combine(); });
        GameManager.instance.onSummonHero += CanCombineHero;
    }

    public void SetHeroData(HeroScriptable mainHero, List<HeroScriptable> subHero)
    { 
        this.mainHero = mainHero;   
        this.subHero = subHero;
        this.heroImage.sprite = mainHero.heroMainImage;
        this.heroName.text = mainHero.heroName;
        CanCombineHero();
    }


    private void Combine()
    {
        if (canCombine)
        {
            StartCoroutine(CombineCoroutine());
        }
    }

    private IEnumerator CombineCoroutine()
    {
        for (int i = 0; i < heroHolders.Count; i++)
        {
            heroHolders[i].Sell();
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.1f);

        Spawner.instance.Summon(mainHero.heroName);
    }

    public void CanCombineHero()
    {
        List<HeroScriptable> heroList = new List<HeroScriptable>(subHero);
        foreach (var heroHolder in Spawner.instance.heroHolders)
        {
            if (heroHolder.Value.holderId.Contains(NetUtils.IsHost() ? "HOST" : "CLIENT"))
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
            combineButton.interactable = true;
            canCombine = true;
        }
        else {
            combineButton.interactable = false;
            canCombine = false;
            heroHolders.Clear();
        }
    }
}

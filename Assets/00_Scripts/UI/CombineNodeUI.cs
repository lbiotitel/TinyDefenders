using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombineNodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private Image heroImage;
    [SerializeField] private Button combineButton;

    private HeroScriptable mainHero;
    private List<HeroScriptable> subHeroList;
    private List<HeroHolder> combineHeroHolderList = new List<HeroHolder>();
    private CombineUI combineUI;
    private bool canCombine = false;



    private void Start()
    {
        combineButton.onClick.AddListener(() => { Combine(); });
        EventManager.instance.onHeroSpawnAction += CanCombineHero;
    }

    public void SetHeroData(HeroScriptable mainHero, List<HeroScriptable> subHero, CombineUI combineUI)
    {
        this.mainHero = mainHero;
        this.subHeroList = subHero;
        this.heroImage.sprite = mainHero.heroMainImage;
        this.heroName.text = mainHero.heroName;
        this.combineUI = combineUI;
    }


    private void Combine()
    {
        if (canCombine)
        {
            for (int i = 0; i < combineHeroHolderList.Count; i++)
            {
                combineHeroHolderList[i].Sell();
            }

            SpawnManager.instance.Spawn(mainHero.heroName);

            CanCombineHero(null);
        }
    }

    public void CanCombineHero(Hero summonHero)
    {

        combineHeroHolderList.Clear();

        List<Hero> currentHeroList = SpawnManager.instance.GetHeroList(NetUtils.GetPlayerNetworkType());

        Dictionary<string, int> combineHeroDic = new Dictionary<string, int>();

        foreach (var hero in subHeroList)
        {
            if (combineHeroDic.ContainsKey(hero.heroName))
            {
                combineHeroDic[hero.heroName]++;
            }
            else {
                combineHeroDic.Add(hero.heroName, 1);
            }
        }

        for (int i = 0; i < currentHeroList.Count; i++)
        {
            if (combineHeroDic.ContainsKey(currentHeroList[i].heroName))
            {
                if (combineHeroDic[currentHeroList[i].heroName] > 0)
                {
                    combineHeroDic[currentHeroList[i].heroName]--;
                    combineHeroHolderList.Add(currentHeroList[i].GetHeroHolder());
                }
            }
        }

        bool isEnough = true;

        foreach (var data in combineHeroDic)
        {
            if (data.Value > 0) isEnough = false;
        }


        if (isEnough)
        {
            combineButton.interactable = true;
            canCombine = true;
        }
        else {
            combineHeroHolderList.Clear();
            combineButton.interactable = false;
            canCombine = false;
        }
    }


    public void FilterNode(HeroTribe filterTribe)
    {
        if (mainHero.heroTribe == filterTribe || filterTribe == HeroTribe.LAST)
        {
            gameObject.SetActive(true);
        }
        else {
            gameObject.SetActive(false);
        }
    } 

    public void OnPointerExit(PointerEventData eventData)
    {
        combineUI.HideHeroInfoUI();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        combineUI.ShowHeroInfoUI(mainHero,subHeroList,transform);
    }
}

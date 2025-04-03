using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI crystalText;

    [SerializeField] private GameObject summonHeroUI;
    [SerializeField] private Transform summonHeroTransform;
    [SerializeField] private GameObject summonHeroNode;

    [SerializeField] private Button monoSummonButton;
    [SerializeField] private Button multiSummonButton;
    [SerializeField] private Button closeSummonButton;

    private List<GameObject> summonHeroList = new List<GameObject>();

    private void Start()
    {
        InitEvent();
        InitButtons();
    }

    private void OnEnable()
    {
        GetCurrentUserAsset();
    }

    private void GetCurrentUserAsset()
    {
        UserData userData = CloudManager.instance.GetUserData();
        goldText.text = userData.gold.ToString();
        crystalText.text = userData.crystal.ToString();
    }

    private void InitButtons()
    {
        monoSummonButton.onClick.AddListener(() => Summon(1));
        multiSummonButton.onClick.AddListener(() => Summon(10));
        closeSummonButton.onClick.AddListener(() => SummonEnd());
    }

    private void Summon(int value)
    {
        int price = value == 1 ? 150 : 1500;
        int currentCrystal = CloudManager.instance.GetUserData().crystal;

        if (currentCrystal < price)
        {
            Debug.Log("재화가 부족");
            return;
        }

        currentCrystal -= price;
        CloudManager.instance.CrystalChangeValue(currentCrystal);
        summonHeroUI.SetActive(true);

        StartCoroutine(SummonHeroCoroutine(value));
    }


    private IEnumerator SummonHeroCoroutine(int value)
    {
        for (int i = 0; i < value; i++)
        {
            HeroRarity rarity = Utils.GetShopSummonRarity();
            HeroScriptable heroScriptable = GetHeroData(rarity);

            if (heroScriptable != null)
            {
                GameObject go = Instantiate(summonHeroNode, summonHeroTransform);
                go.SetActive(false);
                go.transform.Find("Hero Image").GetComponent<Image>().sprite = heroScriptable.heroMainImage;
                go.transform.Find("Hero Circle").GetComponent<Image>().color = Utils.GetRarityColor(rarity);

                summonHeroList.Add(go);
            }

            CloudManager.instance.SetHeroCountValue(heroScriptable.heroName,10);
        }

        CloudManager.instance.SaveUserData();

        for (int i = 0; i < summonHeroList.Count; i++)
        {
            yield return new WaitForSeconds(0.1f);
            summonHeroList[i].SetActive(true);
        }
    }


    private HeroScriptable GetHeroData(HeroRarity rarity)
    {
        //나중에 스크립터블 매니저에서 가져오기
        HeroScriptable[] heroes = Resources.LoadAll<HeroScriptable>("HeroDatas/" + rarity.ToString());
        if (heroes.Length <= 0) return null;

        int random = Random.Range(0, heroes.Length);


        return heroes[random];
    }

    private void SummonEnd()
    {
        summonHeroUI.SetActive(false);

        foreach (var heroNode in summonHeroList)
            Destroy(heroNode);

        summonHeroList.Clear();
    }

    private void InitEvent()
    {
        CloudManager.instance.onCrystalValueChange += ChangeCrystalText;
        CloudManager.instance.onGoldValueChange += ChangeGoldText;
    }

    private void ChangeGoldText()
    {
        goldText.text = CloudManager.instance.GetUserData().gold.ToString();
    }

    private void ChangeCrystalText()
    {
        crystalText.text = CloudManager.instance.GetUserData().crystal.ToString();
    }

    public void BuyCrystal(int value)
    {
        CloudManager.instance.CrystalChangeValue(value);
    }



    private void OnDestroy()
    {
        CloudManager.instance.onCrystalValueChange -= ChangeCrystalText;
        CloudManager.instance.onGoldValueChange -= ChangeGoldText;
    }
}

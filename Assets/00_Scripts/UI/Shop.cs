using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI crystalText;

    [SerializeField] private GameObject summonHeroUI;
    [SerializeField] private Transform summonHeroTransform;
    [SerializeField] private GameObject summonHeroNode;

    [SerializeField] private Button monoSummonButton;
    [SerializeField] private Button multiSummonButton;
    [SerializeField] private Button closeSummonButton;

    

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
        UserData userData = CloudManager.instance.userData;
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
        int currentCrystal = CloudManager.instance.userData.crystal;
        
        if (currentCrystal < price)
        {
            Debug.Log("재화가 부족");
            return;
        }

        for (int i = 0; i < value; i++)
        {
            GameObject go = Instantiate(summonHeroNode, summonHeroTransform);
            Rarity rarity = Utils.GetSummonRarity();
            HeroScriptable heroScriptable = GetHeroData(rarity);

            if (heroScriptable != null)
            {
                go.transform.Find("Hero Image").GetComponent<Image>().sprite = heroScriptable.heroMainImage;
            }
        }

        CloudManager.instance.CrystalChangeValue(currentCrystal - price);
        summonHeroUI.SetActive(true);
    }


    private HeroScriptable GetHeroData(Rarity rarity)
    {
        HeroScriptable[] heroes = Resources.LoadAll<HeroScriptable>("HeroDatas/" + rarity.ToString());
        if(heroes.Length <= 0)return null;

        int random = Random.Range(0,heroes.Length);


        return heroes[random];
    }

    private void SummonEnd()
    {
        summonHeroUI.SetActive(false);

    }

    private void InitEvent()
    {
        CloudManager.instance.onCrystalValueChange += ChangeCrystalText;
        CloudManager.instance.onGoldValueChange += ChangeGoldText;
    }

    private void ChangeGoldText()
    {  
        goldText.text = CloudManager.instance.userData.gold.ToString();
    }

    private void ChangeCrystalText()
    {
        crystalText.text = CloudManager.instance.userData.crystal.ToString();
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

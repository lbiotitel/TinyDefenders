using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroSynergyUINode : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public TextMeshProUGUI synergyNameText;
    public TextMeshProUGUI synergyLevelText;
    [HideInInspector]public HeroSynergySO heroSynergySO;
    private HeroSynergyUI heroSynergyUI;
    private int synergyLevel;

    public void InitSynergyUINode(HeroSynergySO heroSynergySO,HeroSynergyUI heroSynergyUI)
    {
        this.synergyNameText.text = heroSynergySO.synergyName;
        this.synergyLevelText.text = "0";
        this.heroSynergySO = heroSynergySO;
        this.heroSynergyUI = heroSynergyUI;
        gameObject.SetActive(false);
    }

    public void ShowSynergyUINode(int synergyLevel)
    {
        this.heroSynergySO.SetCurrentHeroSynergyLevel(synergyLevel);
        this.synergyLevelText.text = string.Format("LV.{0}", synergyLevel);
        gameObject.SetActive(true);
    }



    public void HideSynergyUINode()
    {
        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        heroSynergyUI.ShowSynergyInfo(transform, heroSynergySO.synergyName, heroSynergySO.GetCurrentSynergyDescription()); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      heroSynergyUI.HideSynergyInfo();
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroSynergyUINode : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public Image synergyImage;
    public TMP_Text synergyLevelText;
    [HideInInspector]public HeroSynergySOBase heroSynergySO;
    private HeroSynergyUI heroSynergyUI;
    private int synergyLevel;

    public void InitSynergyUINode(HeroSynergySOBase heroSynergySO,HeroSynergyUI heroSynergyUI)
    {
        this.heroSynergySO = heroSynergySO;
        this.heroSynergyUI = heroSynergyUI;
        this.synergyImage.sprite = heroSynergySO.GetSynergyImage();

        gameObject.SetActive(false);
    }

    public void ShowSynergyUINode(int synergyLevel)
    {
        this.heroSynergySO.SetCurrentHeroSynergyLevel(synergyLevel);

        synergyLevelText.text = string.Empty;
        string synergyLevelString = string.Empty;

        for (int i = 1; i < heroSynergySO.synergyRequireCount.Length; i++) 
        {
            if(synergyLevel == i) synergyLevelString += string.Format("<color=#00FF00>{0}", heroSynergySO.synergyRequireCount[i]);
            else synergyLevelString += string.Format("<color=#FFFFFF>{0}", heroSynergySO.synergyRequireCount[i]);
            synergyLevelString += string.Format("<color=#FFFFFF>/"); ;
        }

        synergyLevelString = synergyLevelString.Remove(synergyLevelString.Length - 1, 1);

        synergyLevelText.text = synergyLevelString;

        gameObject.SetActive(true);
    }



    public void HideSynergyUINode()
    {
        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string resultDesc = ModifySynergyDescription();
        heroSynergyUI.ShowSynergyInfo(transform, heroSynergySO.GetSynergyName(), resultDesc);
    }

    private string ModifySynergyDescription()
    {
        string resultDesc = string.Empty;
        string synergyDesc = heroSynergySO.GetSynergyDescription();

        string[] splitString = synergyDesc.Split('\n');

        int synergyLevel = heroSynergySO.GetCurrentSynergyLevel();

        for (int i = 0; i < splitString.Length; i++)
        {
            if (synergyLevel == (i + 1))
                resultDesc += "<color=green>" + splitString[i] + "</color>\n";
            resultDesc += splitString[i];
        }

        return resultDesc;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      heroSynergyUI.HideSynergyInfo();
    }
}

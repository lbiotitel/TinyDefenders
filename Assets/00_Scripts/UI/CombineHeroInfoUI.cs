using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombineHeroInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI heroNameText;
    [SerializeField] private TextMeshProUGUI heroRarityText;
    [SerializeField] private Image heroImage;
    [SerializeField] private TextMeshProUGUI heroDescriptionText;
    [SerializeField] private TextMeshProUGUI heroAtkText;
    [SerializeField] private TextMeshProUGUI heroAtkSpeedText;
    [SerializeField] private TextMeshProUGUI heroAtkRangeText;
    [SerializeField] private TextMeshProUGUI heroCriticalRateText;
    [SerializeField] private float yOffset;
    private RectTransform rectTransform;

    private void Awake()
    {
        HideCombineHeroInfoUI();
        rectTransform = GetComponent<RectTransform>();
    }

    public void ShowCombineHeroInfoUI(HeroScriptable heroData, Transform targetTransform)
    {
        heroNameText.text = heroData.heroName;
        heroRarityText.text = heroData.rarity.ToString();
        heroImage.sprite = heroData.heroMainImage;
        heroDescriptionText.text = heroData.GetLocalizationDesc();
        heroAtkText.text = heroData.atk.baseValue.ToString();
        heroAtkSpeedText.text = heroData.atkSpeed.baseValue.ToString();
        heroAtkRangeText.text = heroData.atkRange.baseValue.ToString();
        heroCriticalRateText.text = heroData.criticalRate.baseValue.ToString() + "%";

        ModifyTransform(targetTransform);

        gameObject.SetActive(true);
    }

    private void ModifyTransform(Transform targetTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector3 newPosition = targetTransform.position;
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

        if (!screenRect.Contains(Camera.main.WorldToScreenPoint(corners[0])))
        {
            newPosition.x = Mathf.Max(newPosition.x, rectTransform.rect.width / 2f);
            newPosition.y = Mathf.Max(newPosition.y, rectTransform.rect.height / 2f);
        }
        if (!screenRect.Contains(Camera.main.WorldToScreenPoint(corners[2])))
        {
            newPosition.x = Mathf.Min(newPosition.x, Screen.width - rectTransform.rect.width / 2f);
            newPosition.y = Mathf.Min(newPosition.y, Screen.height - rectTransform.rect.height / 2f);
        }

        rectTransform.position = newPosition + new Vector3(0, yOffset, 0);
    }

    public void HideCombineHeroInfoUI()
    {
        gameObject.SetActive(false);
    }
}

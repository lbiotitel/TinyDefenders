using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroCollectionUINode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI heroNameText;
    [SerializeField] private Image heroImage;
    [SerializeField] private TextMeshProUGUI heroRarityText;
    [SerializeField] private TextMeshProUGUI heroCountText;
    [SerializeField] private Image heroCountSlider;


    public void Init(string heroName, Sprite heroImage, HeroRarity rarity, int heroCount)
    {
        this.heroNameText.text = heroName;
        this.heroImage.sprite = heroImage;

        this.heroImage.color = heroCount == 0 ? Color.black : Color.white;

        this.heroRarityText.text = rarity.ToString();
        this.heroRarityText.color = Utils.GetRarityColor(rarity);

        this.heroCountText.text = heroCount.ToString();

        this.heroCountSlider.fillAmount = (float)heroCount / 100f;
    }
}

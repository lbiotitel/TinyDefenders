using System.Collections.Generic;
using UnityEngine;

public class HeroCollectionUI : MonoBehaviour
{
    [SerializeField] private HeroCollectionUINode heroCollectionUINode;
    [SerializeField] private Transform contentParent;
    private List<HeroCollectionUINode> heroCollectionUINodes = new List<HeroCollectionUINode>();

    private void OnEnable()
    {
        InitHeroCollection();
    }

    private void InitHeroCollection()
    {

        if (heroCollectionUINodes.Count > 0)
        {
            heroCollectionUINodes.ForEach(x => Destroy(x.gameObject));
            heroCollectionUINodes.Clear();
        }

        List<HeroScriptable> heroDatas = SoDataManager.instance.GetAllHeroScriptableData();

        foreach (var data in heroDatas)
        {
            HeroCollectionUINode node = Instantiate(heroCollectionUINode,contentParent);
            node.Init(data.GetLocalizationName(), data.heroMainImage, data.rarity, 1);

            heroCollectionUINodes.Add(node);
        }
    }
}

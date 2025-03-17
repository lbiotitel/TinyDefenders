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

        Dictionary<string,UserData.HeroStoreData> ownHeroDatas = CloudManager.instance.userData.heroDatas;

        foreach (var data in ownHeroDatas)
        {
            HeroCollectionUINode node = Instantiate(heroCollectionUINode,contentParent);
            HeroScriptable heroData = SoDataManager.instance.GetHeroScriptableData(data.Key);
            node.Init(data.Key,heroData.heroMainImage,heroData.rarity, data.Value.heroCount);

            heroCollectionUINodes.Add(node);
        }
    }
}

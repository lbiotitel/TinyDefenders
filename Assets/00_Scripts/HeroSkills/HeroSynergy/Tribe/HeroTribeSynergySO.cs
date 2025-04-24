using UnityEngine;

[CreateAssetMenu(fileName = "HeroTribeSynergySO", menuName = "Scriptable Objects/HeroTribeSynergySO")]
public class HeroTribeSynergySO : HeroSynergySOBase
{
    [SerializeField] private HeroTribe synergyTribe;

    public override void DecreaseHeroCount(HeroScriptable heroData)
    {
        if (heroData.heroTribe == synergyTribe)
        {
            currentHeroCount = Mathf.Max(0, --currentHeroCount);
        }
    }

    public override Sprite GetSynergyImage()
    {
        return GameResourceManager.instance.GetTribeImage(synergyTribe);
    }

    public override void IncreaseHeroCount(HeroScriptable heroData)
    {
        if (heroData.heroTribe == synergyTribe)
        {
            currentHeroCount++;
        }
    }
}

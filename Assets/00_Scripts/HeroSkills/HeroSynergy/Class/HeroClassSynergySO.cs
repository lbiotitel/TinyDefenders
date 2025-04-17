using UnityEngine;

[CreateAssetMenu(fileName = "HeroClassSynergySO", menuName = "Scriptable Objects/HeroClassSynergySO")]
public class HeroClassSynergySO : HeroSynergySOBase
{
    public HeroClass syneryClass;

    public override void DecreaseHeroCount(HeroScriptable heroData)
    {
        if (heroData.heroClass == syneryClass)
        {
            currentHeroCount = Mathf.Max(0, --currentHeroCount);
        }
    }

    public override Sprite GetSynergyImage()
    {
        return GameResourceManager.instance.GetClassImage(syneryClass);
    }

    public override void IncreaseHeroCount(HeroScriptable heroData)
    {
        Debug.Log("호출1");
        if (heroData.heroClass == syneryClass)
        {
        Debug.Log("호출2");
            currentHeroCount++;
        }
    }
}

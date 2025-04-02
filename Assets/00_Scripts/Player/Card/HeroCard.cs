using System.Collections;
using UnityEngine;

public class HeroCard : CardSO
{
    private HeroScriptable heroData;

    public void InitSummonCard(HeroScriptable heroData)
    { 
        this.heroData = heroData;
        this.cardName = heroData.heroName;
        this.cardImage = heroData.heroMainImage;
        this.cardCost = heroData.heroCost;
    }

    public void Summon(Transform cardTransform)
    {
        Spawner.instance.SummonHero(heroData.heroName,cardTransform);
    }

    public HeroScriptable GetHeroData() 
    {
        return heroData;
    }
}

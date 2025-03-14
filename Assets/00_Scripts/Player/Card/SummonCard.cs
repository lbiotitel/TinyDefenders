using System.Collections;
using UnityEngine;

public class SummonCard : CardSO
{
    private HeroScriptable heroData;

    public void InitSummonCard(HeroScriptable heroData)
    { 
        this.heroData = heroData;
        this.cardName = heroData.heroName;
        this.cardImage = heroData.heroMainImage;
    }

    public void Summon(Transform cardTransform)
    {
        Spawner.instance.SummonHero(heroData,cardTransform);
    }
}

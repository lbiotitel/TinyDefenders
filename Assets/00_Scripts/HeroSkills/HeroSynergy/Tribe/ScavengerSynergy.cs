using UnityEngine;

public class ScavengerSynergy : HeroSynergyBase
{
    [SerializeField] private float[] goldBonus;
    [SerializeField] private float getGoldCardRate;
    [SerializeField] private string getGoldCardName;
    public override void HeroSynergyActive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.AddStatModifire(synergyNetworkType, PlayerStatType.SkillAcc, goldBonus[level - 1]);
        if (level == 3)
        { 
            EventManager.instance.onDrawCardAction += ScavengerDrawGoldCard;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if(level < 1)return;
        PlayerStatManager.instance.AddStatModifire(synergyNetworkType, PlayerStatType.SkillAcc, goldBonus[level - 1]);
        if (level == 3)
        {
            EventManager.instance.onDrawCardAction -= ScavengerDrawGoldCard;
        }
    }

    private void ScavengerDrawGoldCard()
    {
        float randomValue = Random.Range(0,100f);

        if (randomValue <= getGoldCardRate)
        {
            CardManager.instance.DrawCard(getGoldCardName);
        }
    }
}

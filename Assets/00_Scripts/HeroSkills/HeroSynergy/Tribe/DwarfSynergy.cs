using UnityEngine;

public class DwarfSynergy : HeroSynergyBase
{
    [SerializeField] private int drawSpellCardSynergyLevel = 1;
    [SerializeField] private int drawHeroCardSynergyLevel = 2;

    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;

        ModifyDrawCardCount(1);
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (!CheckSynergyLevel(level)) return;

        ModifyDrawCardCount(-1);
    }


    private void ModifyDrawCardCount(int value)
    {
        if (currentSynergyLevel == drawSpellCardSynergyLevel)
        {
            NetworkRpcManager.instance.ModifyClientDrawSpellCardCountClientRpc((int)synergyNetworkType,value);
        }

        if (currentSynergyLevel == drawHeroCardSynergyLevel)
        {
            NetworkRpcManager.instance.ModifyClientDrawHeroCardCountClientRpc((int)synergyNetworkType, value);
        }
    }

}

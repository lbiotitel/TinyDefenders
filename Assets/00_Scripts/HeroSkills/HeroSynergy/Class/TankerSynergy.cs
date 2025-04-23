using UnityEngine;

public class TankerSynergy : HeroSynergyBase
{
    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
    }

    public override void HeroSynergyDeactive(int level)
    {
        if(!CheckSynergyLevel(level))return;
    }


    private void TankerSynergyLogic()
    { 
        
    }
}

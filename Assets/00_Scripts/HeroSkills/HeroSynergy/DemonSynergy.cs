using UnityEngine;

public class DemonSynergy : HeroSynergyBase
{
    [SerializeField] private float[] heroAtk;

    public override void HeroSynergyActive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.AddAtkModifire(playerNetworkType,heroAtk[level - 1]);
        if(level == 4)
        {
            
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.RemoveAtkModifire(playerNetworkType, heroAtk[level - 1]);
        if (level == 4)
        { 
            
        }
    }

    private void DemonExplosion()
    { 
        
    }
}

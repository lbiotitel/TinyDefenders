using UnityEngine;

public class HumanSynergy : HeroSynergyBase
{
    [SerializeField] private int[] expAmount;

    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;

        EventManager.instance.onWaveFinishAction += HumanSynergyLogic;
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (!CheckSynergyLevel(level)) return;

        EventManager.instance.onWaveFinishAction -= HumanSynergyLogic;
    }


    private void HumanSynergyLogic()
    {
        switch (synergyNetworkType)
        {
            case PlayerNetworkType.HOST:
                PlayerStatManager.instance.AddIngameExp(expAmount[currentSynergyLevel]);
                break;
            case PlayerNetworkType.CLIENT:
                NetworkRpcManager.instance.AddPlayerExpClientRpc(expAmount[currentSynergyLevel]);
                break;
        }
    }
}

using Unity.Netcode;
using UnityEngine;

public class RogueSynergy : HeroSynergyBase 
{
    [SerializeField] private float[] criModifireValue;
    [SerializeField] private float excutionHpPercent;

    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
        PlayerStatManager.instance.AddCriticalRateModifire(synergyNetworkType, criModifireValue[currentSynergyLevel]);
        if (level == 3)
        {
            EventManager.instance.onHeroAttackToMonsterAction += RogueSynergyLogic;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (!CheckSynergyLevel(level)) return;
        PlayerStatManager.instance.RemoveCriticalRateModifire(synergyNetworkType, criModifireValue[currentSynergyLevel]);
        if (level == 3)
        {
            EventManager.instance.onHeroAttackToMonsterAction -= RogueSynergyLogic;
        }
    }

    private void RogueSynergyLogic(Hero hero, Monster monster)
    {
        if (synergyNetworkType != PlayerNetworkType.HOST) return;

        double hpPercent = monster.hp / monster.maxHp;

        if (excutionHpPercent >= hpPercent)
        {
            NetworkRpcManager.instance.SpawnSynergyParticleClientRpc(synergyName,monster.transform.position);
            monster.DeadClientRpc();
        }
    }
}

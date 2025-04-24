using UnityEngine;

public class ElfSynergy : HeroSynergyBase
{
    [SerializeField] private float heroUpgradeProbability;

    public override void HeroSynergyActive(int level)
    {
        if (!CheckSynergyLevel(level)) return;

        if (currentSynergyLevel == 1)
        {
            ModifyHeroLevelUpRequireCount(-1);
        }

        if (currentSynergyLevel == 2)
        {
            ModifyHeroLevelUpRequireCount(-1);
            EventManager.instance.onHeroSpawnAction += ElfSynergyLogic;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (!CheckSynergyLevel(level)) return;

        if (currentSynergyLevel == 1)
        {
            ModifyHeroLevelUpRequireCount(1);
        }

        if (currentSynergyLevel == 2)
        {
            ModifyHeroLevelUpRequireCount(1);
            EventManager.instance.onHeroSpawnAction -= ElfSynergyLogic;
        }
    }

    private void ModifyHeroLevelUpRequireCount(int value)
    {
        switch (synergyNetworkType)
        {
            case PlayerNetworkType.HOST:
                Debug.Log("호출");
                PlayerStatManager.instance.requireCombineHeroCount += value;
                break;

            case PlayerNetworkType.CLIENT:
                NetworkRpcManager.instance.ModifyHeroLevelUpRequireCountClientRpc(value);
                break;
        }
    }


    private void ElfSynergyLogic(Hero hero)
    {
        if (hero.heroOwnerNetworkType != synergyNetworkType) return;

        int level = hero.GetHeroData().heroLevel;

        MathUtils.RandomChanceAction(heroUpgradeProbability, ()=> {
            Debug.Log("호출됨");
            hero.SetHeroLevel(level + 1);
        });
    }
}

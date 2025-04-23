using UnityEngine;

public class DemonSynergy : HeroSynergyBase
{
    [SerializeField] private float[] heroAtk;
    [SerializeField] private float demonExplosionRange;
    [SerializeField] private GameObject demonExplosionPrefab;

    public override void HeroSynergyActive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.AddAtkModifire(synergyNetworkType, heroAtk[level - 1]);
        if (level == 4)
        {
            EventManager.instance.onHeroAttackToMonsterAction += DemonExplosion;
        }
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (level < 1) return;
        PlayerStatManager.instance.RemoveAtkModifire(synergyNetworkType, heroAtk[level - 1]);
        if (level == 4)
        {
            EventManager.instance.onHeroAttackToMonsterAction -= DemonExplosion;
        }
    }

    private void DemonExplosion(Hero hero, Monster monster)
    {

        if (NetUtils.GetPlayerNetworkType() != PlayerNetworkType.HOST) return;
        HeroScriptable heroData = hero.GetHeroData();
        if (heroData.heroTribe != HeroTribe.DEMON) return;

        double heroAtk = heroData.GetHeroAtk() / 2f;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(monster.transform.position, demonExplosionRange, LayerMask.GetMask(LayerName.Monster));

        NetworkRpcManager.instance.SpawnSynergyParticleClientRpc(synergyName,monster.transform.position);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out Monster targetMonster))
            {
                if (targetMonster == monster) continue;
                if (targetMonster.IsDead()) continue;
                targetMonster.GetDamage(heroAtk, synergyNetworkType,hero.NetworkObjectId,false);
            }
        }
    }

}

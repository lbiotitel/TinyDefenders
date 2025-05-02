using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class Projectile : MonoBehaviour
{
    [SerializeField]private ProjectileSO _projectileSO;
    public ProjectileSO ProjectileSO { get { return _projectileSO; } }

    protected ProjectileMotionBase _projectileMotionBase { get; private set; }
    public DamageData _damageData { get; protected set; }
    private bool isInit = false;

    public virtual void Init(DamageData damageData)
    {

        _damageData = damageData;

        Vector3 startPos = damageData.Hero.transform.position;
        Vector3 endPos = damageData.Target.transform.position;

        switch (_projectileSO.ProjectileMoveType)
        {
            case ProjectileMoveType.STRAIGHT:
                {
                    _projectileMotionBase = InitProjectileMotion<StraightMotion>();
                    _projectileMotionBase.Init(startPos, endPos, _projectileSO,
                        () => AttackMonster());
                }
                break;
            case ProjectileMoveType.BAZIER:
                {
                    _projectileMotionBase = InitProjectileMotion<BazierMotion>();
                    _projectileMotionBase.Init(startPos, endPos, _projectileSO,
                        () => AttackMonster());
                    break;
                }
            case ProjectileMoveType.MELEE:
                {
                    _projectileMotionBase = InitProjectileMotion<MeleeMotion>();
                    MeleeMotion meleeMotion = _projectileMotionBase as MeleeMotion;
                    meleeMotion.Init(endPos, _projectileSO, () => AttackMonster());
                    break;
                }
            case ProjectileMoveType.LASER:
                {
                    _projectileMotionBase = InitProjectileMotion<LaserMotion>();
                    LaserMotion laserMotion = _projectileMotionBase as LaserMotion;
                    laserMotion.Init(startPos, endPos, _projectileSO,
                        () => AttackMonster());
                }
                break;
            case ProjectileMoveType.SPIN:
                _projectileMotionBase = InitProjectileMotion<SpinMotion>();
                SpinMotion spinMotion = _projectileMotionBase as SpinMotion;
                spinMotion.Init(startPos, endPos, _projectileSO,
                    () => AttackMonster());
                break;
        }
    }


    private T InitProjectileMotion<T>() where T : ProjectileMotionBase
    {
        if (!isInit)
        {
            gameObject.AddComponent<T>();
            isInit = true;
        }

        return GetComponent<T>();
    }

    protected void AttackMonster()
    {
        Hero hero = _damageData.Hero;
        Monster monster = _damageData.Target;
        List<DebuffEffect> debuffEffects = _damageData.DebuffEffects;
        double damage = _damageData.Damage;

        if (NetUtils.IsHost() && monster != null && !monster.IsDead())
        {
            switch (_projectileSO.ProjectileDamageType)
            { 
                case ProjectileDamageType.SINGLE:
                    {
                        SingleDamage(hero, monster, debuffEffects, damage);
                    }
                    break;

                case ProjectileDamageType.RANGE:
                    {
                        RangeDamage(hero, debuffEffects, damage);
                    }
                    break;
            }

        }
    }



    #region DamageType Logic


    private static void SingleDamage(Hero hero, Monster monster, List<DebuffEffect> debuffEffects, double damage)
    {
        NetworkRpcManager.instance.AttackMonster(
            hero.heroOwnerNetworkType,
            damage,
            hero,
            monster,
            debuffEffects);
    }

    private void RangeDamage(Hero hero, List<DebuffEffect> debuffEffects, double damage)
    {
        List<Monster> monsterList = SpawnManager.instance.GetMonsterList();

        foreach (var m in monsterList)
        {
            if (m.IsDead()) continue;
            if (Vector3.Distance(m.transform.position, transform.position) <= (int)_projectileSO.ProjectileRangeSize)
            {
                NetworkRpcManager.instance.AttackMonster(
                    hero.heroOwnerNetworkType,
                    damage,
                    hero,
                    m,
                    debuffEffects);
            }
        }
    }



    #endregion



}

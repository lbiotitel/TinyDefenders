using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class Projectile : MonoBehaviour
{
    public ProjectileSO _projectileSO;
    protected ProjectileMotionBase _projectileMotionBase { get; private set; }
    public DamageData _damageData { get; protected set; }

    public virtual void Init(DamageData damageData)
    {
        _damageData = damageData;

        Vector3 startPos = damageData.Hero.transform.position;
        Vector3 endPos = damageData.Target.transform.position;


        switch (_projectileSO.ProjectileMoveType)
        {
            case ProjectileMoveType.STRAIGHT:
                {
                    _projectileMotionBase = gameObject.AddComponent<StraightMotion>();
                    _projectileMotionBase.Init(startPos, endPos, _projectileSO,
                        () => AttackMonster());
                }
                break;
            case ProjectileMoveType.BAZIER:
                {
                    _projectileMotionBase = gameObject.AddComponent<BazierMotion>();
                    _projectileMotionBase.Init(startPos, endPos, _projectileSO,
                        () => AttackMonster());
                    break;
                }
            case ProjectileMoveType.MELEE:
                {
                    _projectileMotionBase = gameObject.AddComponent<MeleeMotion>();
                    MeleeMotion meleeMotion = _projectileMotionBase as MeleeMotion;
                    meleeMotion.Init(endPos, _projectileSO, () => AttackMonster());
                    break;
                }
        }


        switch (_projectileSO.ProjectileDamageType)
        {
            case ProjectileDamageType.SINGLE:
                {

                }
                break;
        }
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
                        NetworkRpcManager.instance.AttackMonster(
                            hero.heroOwnerNetworkType,
                            damage,
                            hero,
                            monster,
                            debuffEffects);
                    }
                    break;

                case ProjectileDamageType.RANGE:


                    break;
            }

        }
    }




}

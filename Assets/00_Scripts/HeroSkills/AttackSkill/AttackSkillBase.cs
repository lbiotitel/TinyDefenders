using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AttackSkillBase : SkillBase
{

    protected override void ActiveSkill()
    {
        if (!skillDelay)
        {
            SkillLogic();
        }
    }


    protected virtual void SkillLogic()
    {

    }

    protected bool CheckMonster()
    {
        List<Monster> monsterList = hero.GetMonsterList();
        
        foreach(var monster in monsterList)
        {
            if (!monster.IsDead()) return true;
        }

        return false;
    } 
}

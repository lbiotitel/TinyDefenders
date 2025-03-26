using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class AttackSkillBase : SkillBase
{

    public override void SkillLogic() {
        hero.AnimatorChange("SKILL", true);
    }

    public override bool CheckSkillCanActive()
    {
        List<Monster> monsters = hero.GetMonsterList();

        int count = 0;
        monsters.ForEach(monster => count += monster.IsDead() == true ? 0 : 1);

        return count > 0;
    }

    protected List<Monster> FindCloseMonster(int count)
    {
        List<Monster> monsterInRangeList = hero.GetMonsterList();
        List<Monster> closeMonsterList = new List<Monster>();

        monsterInRangeList = monsterInRangeList.OrderBy(monster => Vector2.Distance(transform.position, monster.transform.position)).ToList();

        int cnt = 0;
        foreach (var monster in monsterInRangeList)
        {
            if (cnt == count) break;
            if (monster.IsDead()) continue;
            closeMonsterList.Add(monster);
            cnt++;
        }

        return closeMonsterList;
    }


}

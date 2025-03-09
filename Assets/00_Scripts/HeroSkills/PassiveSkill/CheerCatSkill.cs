using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheerCatSkill : PassiveSkillBase
{
    private List<Hero> buffHeroList = new List<Hero>();
    float buffAmount;

    protected override void ActiveSkill()
    {
        List<Hero> heroList = GameManager.instance.GetHeroList();
        buffAmount = heroData.skillData.buffAmount;

        foreach (var hero in heroList)
        {
            if (Vector2.Distance(transform.position, hero.transform.position) <= heroData.skillData.buffRange)
            {
                if (NetUtils.IsMineHero(base.hero, hero))
                {
                    Skill skill;
                    if (hero.TryGetComponent<Skill>(out skill))
                    {
                        skill.skillAcc += buffAmount;
                        buffHeroList.Add(hero);
                    }
                }
            }
        }
    }

    private IEnumerator BuffSkillCoroutine()
    {
        yield return new WaitForSeconds(heroData.skillData.buffTime);

        foreach (var hero in buffHeroList)
        {
            if (hero != null)
            {
                Skill skill;
                if (hero.TryGetComponent<Skill>(out skill))
                {
                    skill.skillAcc -= buffAmount;
                    buffHeroList.Add(hero);
                }
            }
        }

        buffHeroList.Clear();
    }
}

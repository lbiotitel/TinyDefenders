using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheerCatSkill : PassiveSkillBase
{


    protected override void ActiveSkill()
    {
        List<Hero> buffHeroList = new List<Hero>();
        List<Hero> heroList = GameManager.instance.GetHeroList();
        float  buffAmount = heroData.skillData.buffAmount;

        heroList.Remove(base.hero);

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

        StartCoroutine(BuffSkillCoroutine(buffHeroList,buffAmount));
    }

    private IEnumerator BuffSkillCoroutine(List<Hero> heroes,float buffAmount)
    {
        yield return new WaitForSeconds(heroData.skillData.buffTime);

        foreach (var hero in heroes)
        {
            if (hero != null)
            {
                Skill skill;
                if (hero.TryGetComponent<Skill>(out skill))
                {
                    skill.skillAcc -= buffAmount;
                }
            }
        }
    }
}

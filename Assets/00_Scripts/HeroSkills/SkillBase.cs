using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    protected Hero hero;
    protected HeroScriptable heroData;


    public void InitHeroSkill(Hero hero)
    {
        this.hero = hero;
        this.heroData = hero.GetHeroData();
    }

    public abstract void SkillLogic();

    public abstract bool CheckSkillCanActive();
}

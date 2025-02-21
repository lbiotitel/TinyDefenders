using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    private Hero hero;
    private bool skillDelay = false;
    private HeroScriptable heroData;
    [SerializeField] private Image skillFillImage;
    [SerializeField] private GameObject skillCanvas;

    private void Start()
    {
        InitSkillData();
    }

    private void InitSkillData()
    {
        hero = GetComponent<Hero>();
        heroData = hero.GetHeroData();
        if (heroData.skillData.skill == SKILL.NONE)
        {
            skillCanvas.gameObject.SetActive(false);
            Destroy(this);
            return;
        }
        skillCanvas.gameObject.SetActive(true);
    }

    private void Update()
    {
        SkillLogic();
    }

    private void SkillLogic()
    {
        if (heroData.skillData.skillType == SkillType.ACTIVE)
            if (hero.target == null) return;

        if (!skillDelay)
        {
            skillDelay = true;
            StartCoroutine(SkillDelayCoroutine());
            ActiveSkill();
        }
    }

    private void ActiveSkill()
    {
        switch (heroData.skillData.skill)
        {
            case SKILL.TANK_BUNNY_SKILL:
                TankBunnySkill();
                break;
            case SKILL.LOBBER_BUNNY_SKILL:
                LobberBunnySkill();
                break;
        }
    }

    private void TankBunnySkill()
    {
        GameObject go = Instantiate(heroData.skillData.projectilePrefab,
            transform.position,Quaternion.identity);
        go.GetComponent<Projectile>().Init(hero.target.transform,hero);
    }

    private void LobberBunnySkill()
    {
        GameManager.instance.GetGold(2,HostType.CLIENT);
    }


    private IEnumerator SkillDelayCoroutine()
    {
        float t = 0f;
        float coolTime = heroData.skillData.coolTime;

        while (t < coolTime)
        {
            t += Time.deltaTime;
            skillFillImage.fillAmount = (t / coolTime);
            yield return null;
        }
        skillDelay = false;
    }
    
}

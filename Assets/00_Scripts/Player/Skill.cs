using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Skill : MonoBehaviour
{
    protected Hero hero;
    protected bool skillDelay = false;
    protected HeroScriptable heroData;
    [SerializeField] protected Image skillFillImage;
    [SerializeField] protected GameObject skillCanvas;

    public float coolTime;
    public float skillDamage;
    public GameObject projectilePrefab;


    protected void Start()
    {
        InitSkillData();
    }

    protected void InitSkillData()
    {
        hero = GetComponent<Hero>();
        heroData = hero.GetHeroData();
        skillCanvas = transform.GetChild(0).gameObject;
        skillCanvas.SetActive(true);
        skillFillImage = skillCanvas.transform.GetChild(0).GetChild(1).GetComponent<Image>();
    }

    protected void Update()
    {
        SkillLogic();
    }

    protected virtual void SkillLogic() { }


    protected IEnumerator SkillDelayCoroutine()
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

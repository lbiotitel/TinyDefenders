using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SkillBase : NetworkBehaviour
{
    protected Hero hero;
    protected HeroScriptable heroData;
    [SerializeField] protected Image skillFillImage;
    [SerializeField] protected GameObject skillCanvas;
    public float skillAcc = 1f;
    protected bool skillDelay = true;

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
        StartCoroutine(SkillDelayCoroutine());
    }

    protected void Update()
    {
        ActiveSkill();
    }

    protected virtual void ActiveSkill() { }

    protected IEnumerator SkillDelayCoroutine()
    {
        float t = 0f;
        float coolTime = heroData.skillData.coolTime;

        while (t < coolTime)
        {
            t += Time.deltaTime * skillAcc;
            skillFillImage.fillAmount = (t / coolTime);
            yield return null;
        }

        skillDelay = false;
    }

}


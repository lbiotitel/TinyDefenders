using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SkillContainer : NetworkBehaviour
{
    protected Hero hero;
    protected HeroScriptable heroData;
    [SerializeField] protected Image skillFillImage;
    [SerializeField] protected GameObject skillCanvas;

    public float skillAcc = 1f;
    protected bool skillDelay = true;
    private NetworkVariable<float> skillGage = new NetworkVariable<float>(0f);
    SkillBase heroSkill;


    protected void Start()
    {
        InitHeroSkill();
        skillGage.OnValueChanged += OnSkillGageChanged;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        skillGage.OnValueChanged -= OnSkillGageChanged;
    }


    private void InitHeroSkill()
    {
        hero = GetComponent<Hero>();
        heroData = hero.GetHeroData();

        Type classType = Type.GetType(heroData.heroName + "Skill");
        if (classType != null)
        {
            gameObject.AddComponent(classType);
            skillCanvas.SetActive(true);
            StartCoroutine(SkillDelayCoroutine());
            heroSkill = GetComponent<SkillBase>();
            heroSkill.InitHeroSkill(hero);
        }
        else
        {
            skillCanvas.SetActive(false);
            this.enabled = false;
        }
    }


    protected void Update()
    {
        if (IsServer)
        {
            if (!skillDelay)
            {
                if (heroSkill.CheckSkillCanActive())
                {
                    skillDelay = true;
                    StartCoroutine(SkillDelayCoroutine());
                    ActiveSkillClientRpc();
                }
            }
        }
    }

    [ClientRpc]
    private void ActiveSkillClientRpc()
    {
        heroSkill.SkillLogic();
    }


    protected virtual void ActiveSkill() { }

    protected IEnumerator SkillDelayCoroutine()
    {
        if (IsServer)
        {
            float t = 0f;
            float coolTime = heroData.skillData.coolTime;

            while (t < coolTime)
            {
                t += Time.deltaTime * skillAcc;
                skillGage.Value = (t / coolTime);
                yield return null;
            }

            skillDelay = false;
        }
    }

    private void OnSkillGageChanged(float oldValue, float newValue)
    {
        skillFillImage.fillAmount = newValue;
    }

}


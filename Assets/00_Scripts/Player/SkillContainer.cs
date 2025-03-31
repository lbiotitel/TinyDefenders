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

    protected bool skillDelay = true;
    private NetworkVariable<float> skillGage = new NetworkVariable<float>(0f);
    private SkillBase heroSkill;
    private Coroutine skillDelayCoroutine = null;


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
            skillDelayCoroutine = StartCoroutine(SkillDelayCoroutine());
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
                    skillDelayCoroutine = StartCoroutine(SkillDelayCoroutine());
                    ActiveSkillClientRpc();
                    EventManager.instance.onHeroUseSkillAction?.Invoke(hero);
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
            float coolTime = heroData.skillData.coolTime - (heroData.skillData.coolTime * hero.GetHeroSkillAcc() / 100f);

            while (t < coolTime)
            {
                t += Time.deltaTime;
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

    public void ResetSkill()
    {
        StartCoroutine(ResetSkillCoroutine());
    }

    private IEnumerator ResetSkillCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        if (heroSkill.CheckSkillCanActive())
        {
            ActiveSkillClientRpc();
        }
    }
}


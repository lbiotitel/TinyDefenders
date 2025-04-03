using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    public UnityAction<Hero> onHeroUseSkillAction;
    public UnityAction onHeroSynergyChangedAction;
    public UnityAction<Hero> onHeroSummonAction;
    public UnityAction<Hero> onHeroDespawnAction;
    public UnityAction<Hero,Monster> onHeroAttackToMonsterAction;
    public UnityAction onDrawCardAction;


    private void ReleaseHeroAction(ref UnityAction<Hero> unityAction)
    {
        if (unityAction != null)
        {
            foreach (var d in unityAction.GetInvocationList())
            { 
                unityAction -= (UnityAction<Hero>)d;
            }
        }
    }

    private void ReleaseHeroAction(ref UnityAction<Hero, Monster> unityAction)
    {
        if (unityAction != null)
        {
            foreach (var d in unityAction.GetInvocationList())
            {
                unityAction -= (UnityAction<Hero,Monster>)d;
            }
        }
    }

    private void ReleaseAction(ref UnityAction unityAction)
    {
        if (unityAction != null)
        {
            foreach (var d in unityAction.GetInvocationList())
            {
                unityAction -= (UnityAction)d;
            }
        }
    }

    private void OnDestroy()
    {
        ReleaseHeroAction(ref onHeroUseSkillAction);
        ReleaseAction(ref onHeroSynergyChangedAction);
        ReleaseHeroAction(ref onHeroSummonAction);
        ReleaseHeroAction(ref onHeroDespawnAction);
        ReleaseAction(ref onDrawCardAction);
        ReleaseHeroAction(ref onHeroAttackToMonsterAction);
        onHeroUseSkillAction = null;
        instance = null;
    }
}

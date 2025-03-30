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
    public UnityAction onHeroSummonAction;
    public UnityAction onHeroDespawnAction;


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
        ReleaseAction(ref onHeroSummonAction);
        ReleaseAction(ref onHeroDespawnAction);
        onHeroUseSkillAction = null;
        instance = null;
    }
}

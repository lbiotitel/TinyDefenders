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


    public UnityAction<Hero> heroUseSkillAction;


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

    private void OnDestroy()
    {
        ReleaseHeroAction(ref heroUseSkillAction);
        heroUseSkillAction = null;
        instance = null;
    }
}

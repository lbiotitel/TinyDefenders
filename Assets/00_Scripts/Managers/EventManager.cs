using System;
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
    public UnityAction<Hero> onHeroStartSummonAction;
    public UnityAction onHeroEndSummonAction;

    public UnityAction<float,bool> onTimerTickAction;
    public UnityAction onWaveNotClearAction;
    public UnityAction onWaveClearAction;

    public UnityAction<Hero> onHeroDespawnAction;
    public UnityAction<Hero,Monster> onHeroAttackToMonsterAction;
    public UnityAction onDrawCardAction;
    public UnityAction onGameStartAction;
    public UnityAction onBossWaveClear;
    public UnityAction<HeroTribe, PlayerNetworkType> onUpgradeAction;
    public UnityAction<Monster> onMonsterDeadAction;


    private void ReleaseAction<T>(ref UnityAction<T> unityAction) 
    {
        if (unityAction != null)
        {
            foreach (var d in unityAction.GetInvocationList())
            { 
                unityAction -= (UnityAction<T>)d;
            }
        }
    }


    private void ReleaseAction<T1,T2>(ref UnityAction<T1, T2> unityAction)
    {
        if (unityAction != null)
        {
            foreach (var d in unityAction.GetInvocationList())
            {
                unityAction -= (UnityAction<T1,T2>)d;
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
        ReleaseAction(ref onHeroUseSkillAction);
        ReleaseAction(ref onHeroSynergyChangedAction);
        ReleaseAction(ref onHeroSummonAction);
        ReleaseAction(ref onHeroDespawnAction);
        ReleaseAction(ref onDrawCardAction);
        ReleaseAction(ref onGameStartAction);
        ReleaseAction(ref onHeroAttackToMonsterAction);
        ReleaseAction(ref onUpgradeAction);
        ReleaseAction(ref onMonsterDeadAction);
        ReleaseAction(ref onHeroStartSummonAction);
        ReleaseAction(ref onHeroEndSummonAction);
        ReleaseAction(ref onBossWaveClear);
        ReleaseAction(ref onTimerTickAction);
        ReleaseAction(ref onWaveNotClearAction);
        ReleaseAction(ref onWaveClearAction);
        onHeroUseSkillAction = null;
        instance = null;
    }
}

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

    public UnityAction<Hero, HeroStatType, float> onHeroAddStatModifireAction;
    public UnityAction<Hero, HeroStatType, float> onHeroRemoveStatModifireAction;

    public UnityAction<int, int, float> onPlayerAddStatModifireAction;
    public UnityAction<int, int, float> onPlayerRemoveStatModifireAction;

    public UnityAction<float,bool> onTimerTickAction;
    public UnityAction onWaveNotClearAction;
    public UnityAction onWaveClearAction;
    public UnityAction onWaveFinishAction;

    public UnityAction<int,int> onExpLevelChangedAction;
    public UnityAction<int> onLevelChagendAction;

    public UnityAction<RelicSO> onRelicSelectAction;
    public UnityAction<PlayerNetworkType> onRelicSelectFinishAction;
    public UnityAction<Hero> onHeroDespawnAction;

    public UnityAction<Hero,Monster> onHeroAttackToMonsterAction;
    public UnityAction<Monster> onMonsterDeadAction;
    public UnityAction<DamageEvent> onMonsterBeforeTakeDamageAction;
    public UnityAction<Hero,Monster> onBeforeAttackMonsterAction;

    public UnityAction<int> onMaxHeroCountChangedAction;

    public UnityAction onDrawCardAction;
    public UnityAction onGameStartAction;
    public UnityAction onBossWaveClear;


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

    private void ReleaseAction<T1, T2, T3>(ref UnityAction<T1, T2, T3> unityAction)
    {
        if (unityAction != null)
        {
            foreach (var d in unityAction.GetInvocationList())
            {
                unityAction -= (UnityAction<T1, T2, T3>)d;
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
        ReleaseAction(ref onMonsterDeadAction);
        ReleaseAction(ref onHeroStartSummonAction);
        ReleaseAction(ref onHeroEndSummonAction);
        ReleaseAction(ref onBossWaveClear);
        ReleaseAction(ref onTimerTickAction);
        ReleaseAction(ref onWaveNotClearAction);
        ReleaseAction(ref onWaveClearAction);
        ReleaseAction(ref onExpLevelChangedAction);
        ReleaseAction(ref onLevelChagendAction);
        ReleaseAction(ref onWaveFinishAction);
        ReleaseAction(ref onRelicSelectAction);
        ReleaseAction(ref onRelicSelectFinishAction);
        ReleaseAction(ref onMonsterBeforeTakeDamageAction);
        ReleaseAction(ref onBeforeAttackMonsterAction);
        ReleaseAction(ref onHeroAddStatModifireAction);
        ReleaseAction(ref onHeroRemoveStatModifireAction);
        ReleaseAction(ref onPlayerAddStatModifireAction);
        ReleaseAction(ref onPlayerRemoveStatModifireAction);

        instance = null;
    }
}

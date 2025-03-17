using System;
using UnityEngine;


[CreateAssetMenu(fileName = "ActiveCardSO", menuName = "Scriptable Objects/ActiveCardSO")]
public class ActiveCardSO : CardSO
{
    [Header("Active Card Property")]
    public float atkRange;
    public int value;
    public CardAbilityBase activePrefab;

    protected override void OnEnable()
    {
        base.OnEnable();
    }


    public void UseCard(Vector2 targetPos)
    {
        CardAbilityBase cardAbilityBase = Instantiate(activePrefab);
        cardAbilityBase.InitCardAbility(this);
        cardAbilityBase.ActiveAbility(targetPos);
    }
}

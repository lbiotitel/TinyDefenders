using System;
using UnityEngine;


[CreateAssetMenu(fileName = "ActiveCardSO", menuName = "Scriptable Objects/ActiveCardSO")]
public class ActiveCardSO : CardSO
{
    [Header("Active Card Property")]
    [SerializeField] private float atkRange;
    [SerializeField] private int value;
    [SerializeField] private CardAbilityBase activePrefab;

    protected override void OnEnable()
    {
        base.OnEnable();
    }


    public void UseCard()
    {
        CardAbilityBase cardAbilityBase = Instantiate(activePrefab);
        cardAbilityBase.InitCardAbility(this);
        cardAbilityBase.ActiveAbility();
    }
}

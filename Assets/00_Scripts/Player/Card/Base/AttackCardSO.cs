using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


[CreateAssetMenu(fileName = "AttackCardSO", menuName = "Scriptable Objects/AttackCardSO")]
public class AttackCardSO : CardSO
{
    [Header("Active Card Property")]
    public float atkRange;
    public double damage;
    public int value;
    public CardAbilityBase activePrefab;

    [Header("Debuff")]
    public List<DebuffEffect> debuffEffects;

    protected override void OnEnable()
    {
        base.OnEnable();
        cardType = CardType.ATTACK;
    }


    public void UseCard(Vector2 targetPos)
    {
        NetworkRpcManager.instance.SpawnCardAblilityServerRpc(targetPos,cardName);
    }
}

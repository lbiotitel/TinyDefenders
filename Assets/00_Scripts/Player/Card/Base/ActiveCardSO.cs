using System;
using Unity.Netcode;
using UnityEngine;


[CreateAssetMenu(fileName = "ActiveCardSO", menuName = "Scriptable Objects/ActiveCardSO")]
public class ActiveCardSO : CardSO
{
    [Header("Active Card Property")]
    public float atkRange;
    public double damage;
    public int value;
    public CardAbilityBase activePrefab;

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

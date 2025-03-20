using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UtilityCardSO", menuName = "Scriptable Objects/UtilityCardSO")]
public class UtilityCardSO : CardSO
{
    [Header("Utility Card Property")]
    public CardAbilityBase activePrefab;
    public List<int> values;

    protected override void OnEnable()
    {
        base.OnEnable();
        cardType = CardType.UTILITY;
    }


    public void UseCard()
    {
        CardAbilityBase cardAbilityBase = Instantiate(activePrefab);
        cardAbilityBase.InitCardAbility(this);
        cardAbilityBase.ActiveAbility();
    }
}

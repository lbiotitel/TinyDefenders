using UnityEngine;

[CreateAssetMenu(fileName = "ActiveCard", menuName = "Scriptable Objects/ActiveCard")]
public class ActiveCard : CardSO
{
    [Header("Active Card Property")]
    [SerializeField] private float atkRange;
    [SerializeField] private int value;
    [SerializeField] private CardAbilityBase cardAbility;

    private void OnEnable()
    {
        if (cardAbility != null)
        {
            cardAbility.InitCardAbility(this);
        }
    }

    public void UseCard()
    {
        if (cardAbility != null)
        {
            cardAbility.ActiveAbility();
        }
    }
}

using UnityEngine;

public class CardAbilityBase : MonoBehaviour
{
    CardSO cardSO;

    public void InitCardAbility(CardSO cardSO)
    {
        this.cardSO = cardSO;
    }

    public virtual void ActiveAbility()
    { 
        
    }
}

using Unity.Netcode;
using UnityEngine;

public class CardAbilityBase : MonoBehaviour
{
    protected CardSO cardSO;

    protected void OnValidate()
    {
        gameObject.name = GetType().Name;  
    }

    public void InitCardAbility(CardSO cardSO)
    {
        this.cardSO = cardSO;
    }

    public virtual void ActiveAbility()
    { 
        
    }
}

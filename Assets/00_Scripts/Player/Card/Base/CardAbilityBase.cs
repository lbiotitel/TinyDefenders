using Unity.Netcode;
using UnityEngine;

public abstract class CardAbilityBase : MonoBehaviour
{
    protected CardSO cardSO;
    [SerializeField] protected GameObject cardParticlePrefab;

    protected void OnValidate()
    {
        gameObject.name = GetType().Name;  
    }

    public void InitCardAbility(CardSO cardSO)
    {
        this.cardSO = cardSO;
    }

    public abstract void ActiveAbility();
}

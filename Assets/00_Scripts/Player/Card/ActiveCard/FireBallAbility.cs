using UnityEngine;

public class FireBallAbility : CardAbilityBase
{
    [SerializeField] private GameObject fireBallPrefab;


    public override void ActiveAbility()
    {
        Debug.Log("Fire ball »ý¼º");
    }
}

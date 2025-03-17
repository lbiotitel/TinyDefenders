using UnityEngine;
using UnityEngine.UI;

public class FireBallAbility : CardAbilityBase
{
    [SerializeField] private GameObject fireBallPrefab;
    [SerializeField] private LayerMask monsterLayer;
    private Vector2 targetPos;

    public override void ActiveAbility(Vector2 targetPos)
    {
        ActiveCardSO activeCardSO = cardSO as ActiveCardSO;

        Collider2D[] enemiesInRange =
               Physics2D.OverlapCircleAll(targetPos, activeCardSO.atkRange, monsterLayer);

        if (enemiesInRange.Length > 0)
        {
            GameObject fireBall = Instantiate(fireBallPrefab,targetPos + new Vector2(0,5),Quaternion.identity);
            
        }
    }
}

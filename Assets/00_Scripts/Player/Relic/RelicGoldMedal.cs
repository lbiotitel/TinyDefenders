using System.Collections;
using UnityEngine;

public class RelicGoldMedal : RelicBase
{
    [SerializeField] private int goldAmount = 0;
    public override void ActiveRelic()
    {
        EventManager.instance.onMonsterDeadAction += GetGold;
    }

    private void GetGold(Monster monster)
    {
        StartCoroutine(GetGoldCoroutine());
    }

    private IEnumerator GetGoldCoroutine()
    { 
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.GetGold(goldAmount);
    }
}

using UnityEngine;

public class RelicGoldBag : RelicBase
{
    [SerializeField] private int goldAmount = 0;

    public override void ActiveRelic()
    {
        EventManager.instance.onGameStartAction += GetGold;
    }

    private void GetGold()
    {
        GameManager.instance.GetGold(goldAmount);
    }
}

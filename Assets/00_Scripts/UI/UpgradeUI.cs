using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private UpgradeUINode[] upgradeUINodes;
    [SerializeField] private DrawCardRateUINode drawCardRateUINode;    
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();   
    }


    public void ClickHeroUpgradeButton(HeroTribe heroTribe)
    {
        NetworkRpcManager.instance.UpgradeHeroServerRpc((int)heroTribe, NetUtils.IsHost());
    }

    public void ClickDrawCardRateUpgrade()
    {
        PlayerStatManager.instance.UpgradeCardDrawRate(NetUtils.GetPlayerNetworkType());
    }

    public void RedrawUpgradeUINodes()
    {
        for (int i = 0; i < upgradeUINodes.Length; i++)
        {
            upgradeUINodes[i].InitUpgradeUI();
        }

        drawCardRateUINode.InitUpgradeUI();
    }



    public void ShowUpgradeUI()
    { 
        gameObject.SetActive(true);
    }

    public void HideUpgradeUI()
    {
        animator.SetTrigger("HIDE"); 
    }
}

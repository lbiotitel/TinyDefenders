using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private UpgradeUINode[] upgradeUINodes; 
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();   
    }


    public void ClickHeroUpgradeButton(HeroTribe heroTribe)
    {
        NetworkRpcManager.instance.UpgradeHeroServerRpc((int)heroTribe, NetUtils.IsHost());
        EventManager.instance.onUpgradeAction?.Invoke(heroTribe,NetUtils.GetPlayerNetworkType());
    }

    public void RedrawUpgradeUINodes()
    {
        for (int i = 0; i < upgradeUINodes.Length; i++)
        {
            upgradeUINodes[i].InitUpgradeUI();
        }
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

using UnityEngine;

public class SelectRelicCardUI : MonoBehaviour
{
    [SerializeField] private SelectRelicCardNodeUI[] selectRelicCardNodeUIs;

    private void Start()
    {
        EventManager.instance.onRelicSelectAction += RelicSelected;
    }


    public void ShowSelectRelicCardUI()
    {
        gameObject.SetActive(true);

        for (int i = 0; i < selectRelicCardNodeUIs.Length; i++)
        {
            RelicSO relicData = GameResourceManager.instance.GetRelicDataByRarity(RelicRarity.Normal);
            selectRelicCardNodeUIs[i].InitRelic(relicData);
        }
    }


    public void AnimationEndTrigger()
    {
        for (int i = 0; i < selectRelicCardNodeUIs.Length; i++)
        {
            selectRelicCardNodeUIs[i].PlayRarityParticle();
        }
    }

    public void HideSelectRelicUI()
    {
        gameObject.SetActive(false);

        for (int i = 0; i < selectRelicCardNodeUIs.Length; i++)
        {
            selectRelicCardNodeUIs[i].StopRarityParticle();
        }
    }


    private void RelicSelected(RelicSO relicSO)
    {
        HideSelectRelicUI();
        relicSO.InitRelicSO(NetUtils.GetPlayerNetworkType());
        EventManager.instance.onRelicSelectFinishAction?.Invoke(NetUtils.GetPlayerNetworkType());
    }
}

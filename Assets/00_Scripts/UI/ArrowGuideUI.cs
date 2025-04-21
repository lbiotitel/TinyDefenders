using System.Collections;
using UnityEngine;

public class ArrowGuideUI : MonoBehaviour
{
    [SerializeField] private GameObject hostArrowGuide;
    [SerializeField] private GameObject clientArrowGuide;
    [SerializeField] private float lifeTime;

    private void Start()
    {
        HideArrowGuide();
        EventManager.instance.onGameStartAction += ShowArrowGuide;
    }

    private void ShowArrowGuide()
    {
        PlayerNetworkType playerNetworkType = NetUtils.GetPlayerNetworkType();

        switch (playerNetworkType)
        {
            case PlayerNetworkType.HOST:
                hostArrowGuide.SetActive(true);
                break;
            case PlayerNetworkType.CLIENT:
                clientArrowGuide.SetActive(true);
                break;
        }

        StartCoroutine(HideArrowGuideCoroutine());
    }

    private void HideArrowGuide()
    {
        hostArrowGuide.SetActive(false);
        clientArrowGuide.SetActive(false);
    }

    private IEnumerator HideArrowGuideCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);
        HideArrowGuide();
    }
}

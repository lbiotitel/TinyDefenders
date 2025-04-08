using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RelicUINode : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    [SerializeField] private Image relicImage;
    [SerializeField] private GameObject lockObject;
    [SerializeField] private string relicOnSFX;
    [SerializeField] private GameObject relicOnObject;
    private RelicSO relicSO;
    private PlayerRelicUI playerRelicUI;

    public void InitRelic(RelicSO relicSO,PlayerRelicUI playerRelicUI)
    {
        this.relicSO = relicSO;
        this.relicImage.sprite = relicSO.relicImage;
        this.playerRelicUI = playerRelicUI;

        relicOnObject.SetActive(false);

        CheckLock();
    }

    private void CheckLock()
    {
        int userLevel = CloudManager.instance.GetUserData().userLevel;

        if (relicSO.IsEnoughLevel(userLevel))
        {
            relicSO.UnlockRelic();
            lockObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!relicSO.IsUnlock()) return;

        int relicPoint = CloudManager.instance.GetUserData().relicPoint;

        if (!relicSO.IsRelicOn())
        {
            if (relicSO.IsEnoughPoint(relicPoint))
            {
                SoundManager.instance.PlaySfxSound(relicOnSFX, SFXTYPE.UISOUND);
                relicPoint -= relicSO.requirePoint;
                CloudManager.instance.GetUserData().relicPoint = relicPoint;
                playerRelicUI.onRelicPointChanged?.Invoke(relicPoint);
                RelicOn();
            }
        }
        else
        {
            relicPoint += relicSO.requirePoint;
            CloudManager.instance.GetUserData().relicPoint = relicPoint;
            playerRelicUI.onRelicPointChanged?.Invoke(relicPoint);
            RelicOff();
        }
    }

    public void RelicOn()
    {
        relicOnObject.SetActive(true);
        relicSO.RelicOn();
    }

    public void RelicOff()
    {
        relicOnObject.SetActive(false);
        relicSO.RelicOff();
    }


    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        playerRelicUI.ShowRelicInfoUI(transform ,relicSO.relicName, relicSO.relicDesc, relicSO.requirePoint,relicSO.requireLevel);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        playerRelicUI.HideRelicInfoUI();
    }
}

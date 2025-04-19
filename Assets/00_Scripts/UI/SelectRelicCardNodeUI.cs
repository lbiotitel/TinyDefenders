using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectRelicCardNodeUI : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private TMP_Text relicNameText;
    [SerializeField] private Image relicImage;
    [SerializeField] private TMP_Text relicDescText;
    [SerializeField] private ParticleSystem rarityParticle;
    private RelicSO relicSO;



    public void InitRelic(RelicSO relicSO)
    {
        this.relicSO = relicSO;
        relicNameText.text = relicSO.relicName;
        relicImage.sprite = relicSO.relicImage;
        relicDescText.text = relicSO.relicDesc;
    }

    public void StopRarityParticle()
    {
        rarityParticle.Stop();
    }

    public void PlayRarityParticle()
    {
        rarityParticle.Play();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventManager.instance.onRelicSelectAction?.Invoke(relicSO);
    }
}

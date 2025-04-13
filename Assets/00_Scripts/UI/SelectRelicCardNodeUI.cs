using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectRelicCardNodeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text relicNameText;
    [SerializeField] private Image relicImage;
    [SerializeField] private TMP_Text relicDescText;
    [SerializeField] private ParticleSystem rarityParticle;
    private RelicSO relicSO;



    public void InitRelic(RelicSO relicSO)
    {
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

}

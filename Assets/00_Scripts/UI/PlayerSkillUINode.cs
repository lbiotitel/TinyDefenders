using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillUINode : MonoBehaviour
{
    [SerializeField] private TMP_Text playerSkillNameText;
    [SerializeField] private TMP_Text playerSkillPointText;
    [SerializeField] private TMP_Text playerSkillDescText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image[] playerSkillPassiveImages;

    [SerializeField] private Button playerSkillAddButton;
    [SerializeField] private Button playerSkillSubtractButton;
    [SerializeField] private string buttonSFX;
    private PlayerSkillSO playerSkillSO;


    public void InitPlayerSkillUINode(PlayerSkillSO playerSkillSO)
    {
        this.playerSkillSO = playerSkillSO;

        playerSkillNameText.text = playerSkillSO.skillName;
        playerSkillPointText.text = string.Format("LV.{0}", playerSkillSO.skillLevel);

        InitSkillPassiveImage(playerSkillSO);
        InitBackgroundColor(playerSkillSO.playerSkillType);

        playerSkillAddButton.onClick.AddListener(() =>
        {
            SoundManager.instance.PlaySfxSound(buttonSFX, SFXTYPE.UISOUND);
            AddSkillPoint();
        });

        playerSkillSubtractButton.onClick.AddListener(() =>
        {
            SoundManager.instance.PlaySfxSound(buttonSFX, SFXTYPE.UISOUND);
            SubtractSkillPoint();
        });
    }

    private void InitBackgroundColor(PlayerSkillType playerSkillType)
    {
        switch (playerSkillType)
        {
            case PlayerSkillType.FURY:
                backgroundImage.color = Color.red;
                break;
            case PlayerSkillType.FLASH:
                backgroundImage.color = Color.blue;
                break;
            case PlayerSkillType.FATAL:
                backgroundImage.color = Color.green;
                break;
            case PlayerSkillType.SPREAD:
                backgroundImage.color = Color.cyan;
                break;
            case PlayerSkillType.WEALTH:
                backgroundImage.color = Color.yellow;
                break;
            case PlayerSkillType.CHAOS:
                backgroundImage.color = Color.gray;
                break;
        }
    }

    private void InitSkillPassiveImage(PlayerSkillSO playerSkillSO)
    {
        int index = 0;
        foreach (var passiveImage in playerSkillSO.passiveImages)
        {
            playerSkillPassiveImages[index++].sprite = passiveImage;
        }
    }

    private void AddSkillPoint()
    {
        if (playerSkillSO.AddSkillPoint())
        {

            playerSkillPointText.text = string.Format("LV.{0}", playerSkillSO.skillLevel);
        }

    }

    private void SubtractSkillPoint()
    {
        if (playerSkillSO.SubtractSkillPoint())
        {
            playerSkillPointText.text = string.Format("LV.{0}", playerSkillSO.skillLevel);
        }
    }
}

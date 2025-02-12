using TMPro;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI monsterCountText;
    [SerializeField] private TextMeshProUGUI summonGoldText;
    [SerializeField] private TextMeshProUGUI currentGoldText;

    [SerializeField] private Animator assetAnimator;


    private void Start()
    {
        InitEvent();
    }

    private void InitEvent()
    {
        GameManager.instance.onGoldGet += GoldAnimation;
    }

    private void Update()
    {
        DrawMainUI();
    }

    private void DrawMainUI()
    {
        monsterCountText.text = 
            GameManager.instance.currentMonsterCount.ToString() + "/" + GameManager.instance.maxMonsterCount.ToString();
        summonGoldText.text = GameManager.instance.summonGold.ToString();
        currentGoldText.text = GameManager.instance.gold.ToString();
        summonGoldText.color = GameManager.instance.gold >= GameManager.instance.summonGold ? Color.white : Color.red;
    }

    private void GoldAnimation()
    {
        assetAnimator.SetTrigger("GOLDGET");
    }
}

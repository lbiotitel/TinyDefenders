using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ArrowOptionUI : MonoBehaviour
{
    [SerializeField] private List<string> options = new List<string>();
    [SerializeField] private TextMeshProUGUI currentOptionText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    private int currentIndex = 0;
    public UnityAction<int> onValueChanged { get; set; }

    private void Awake()
    {
        nextButton.onClick.AddListener(NextOptions);
        prevButton.onClick.AddListener(PrevOptions);
    }

    private void Start()
    {
        InitOptionText();
    }

    private void InitOptionText()
    {
        if (options.Count > 0)
            currentOptionText.text = options[currentIndex];
    }

    private void NextOptions()
    {
        currentIndex = (currentIndex + 1) % options.Count;
        ShowOptionText();
        onValueChanged?.Invoke(GetCurrentIndex());
        SoundManager.instance.PlaySfxSound("OptionButtonSFX",SFXTYPE.UI);
    }

    private void PrevOptions()
    {
        currentIndex = currentIndex - 1;
        if (currentIndex < 0) currentIndex = options.Count - 1;
        ShowOptionText();
        onValueChanged?.Invoke(GetCurrentIndex());
        SoundManager.instance.PlaySfxSound("OptionButtonSFX", SFXTYPE.UI);
    }

    private void ShowOptionText()
    {
        currentOptionText.text = options[currentIndex].ToString();
    }


    public void SetOptions(List<string> options)
    {
        this.options = new List<string>(options);
    }

    public void SetIndex(int index)
    {
        currentIndex = index;
        ShowOptionText();
    }

    public void ClearOptions()
    { 
        options.Clear();
        currentIndex = 0;
    }

    public int GetCurrentIndex() {  return currentIndex; }
    public string GetCurrentValue() { return options[currentIndex]; }


    private void OnDestroy()
    {
        onValueChanged = null;
    }
}
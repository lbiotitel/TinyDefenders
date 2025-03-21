using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{

    [SerializeField] public float baseValue;
    private List<float> modifiers = new List<float>();

    public float GetValue()
    {
        RemoveEmpty();
        float finalValue = 0f;
        foreach (float addStat in modifiers)
        {
            finalValue += addStat;
        }
        RemoveEmpty();
        if (finalValue == 0f) return baseValue;
        return baseValue + (finalValue / 100f) * baseValue;
    }

    private void RemoveEmpty()
    {
        for (int i = 0; i < modifiers.Count; i++)
        {
            if (modifiers[i] == 0f)
            {
                modifiers.RemoveAt(i--);
            }
        }
    }

    public void RemoveAllModifiers()
    {
        modifiers.Clear();
    }

    public void AddModifire(float modifier)
    {
        modifiers.Add(modifier);
    }

    public void RemoveModifire(float modifier)
    {
        modifiers.Remove(modifier);
    }
}
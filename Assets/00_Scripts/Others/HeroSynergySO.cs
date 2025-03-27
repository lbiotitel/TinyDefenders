using UnityEngine;

[CreateAssetMenu(fileName = "HeroSynergySO", menuName = "Scriptable Objects/HeroSynergySO")]
public class HeroSynergySO : ScriptableObject
{
    public HeroTribe synergyTribe;
    public string synergyName;
    [TextArea] public string synergyDescription;

    public int currentTribeCount = 0;
    public int[] synergyRequireTribeCount;
    public double[] synergyValue;
    private int currentSynergyLevel = 0;

    public int CheckSynergyLevel()
    {
        for (int i = 0; i < synergyRequireTribeCount.Length; i++)
        {
            if (currentTribeCount > synergyRequireTribeCount[i])
            {
                break;
            }
            else {
                currentSynergyLevel = i;
            }
        }

        return currentSynergyLevel;
    }
}

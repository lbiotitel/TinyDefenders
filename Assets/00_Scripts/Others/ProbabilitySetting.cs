using UnityEngine;

[CreateAssetMenu(fileName = "ProbabilitySetting", menuName = "Scriptable Objects/ProbabilitySetting")]
public class ProbabilitySetting : ScriptableObject
{
    [System.Serializable]
    public class ProbabilityNode {
        public float probability;
        public HeroRarity rarity;
    }

    public ProbabilityNode[] probabilityNode;
    [HideInInspector]public float totalWeight;

    private void OnEnable()
    {
        SumWeight();
    }

    private void SumWeight()
    {

        for (int i = 0; i < probabilityNode.Length; i++)
        {
            totalWeight += probabilityNode[i].probability;
        }
    }


    public HeroRarity GetRarity()
    {
        float random = Random.Range(0, totalWeight);

        for (int i = 0; i < probabilityNode.Length; i++)
        {
            if (random < probabilityNode[i].probability)
            {
                return probabilityNode[i].rarity;
            }
            else
            {
                random -= probabilityNode[i].probability;
            }
        }

        return HeroRarity.Normal;
    }
}

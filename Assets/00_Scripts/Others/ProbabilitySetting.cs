using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProbabilityLevel
{
    public List<ProbabilitySetting.ProbabilityNode> nodes = new List<ProbabilitySetting.ProbabilityNode>();
}

[CreateAssetMenu(fileName = "ProbabilitySetting", menuName = "Scriptable Objects/ProbabilitySetting")]
public class ProbabilitySetting : ScriptableObject
{
    [System.Serializable]
    public class ProbabilityNode
    {
        public float probability;
        public HeroRarity rarity;
    }

    [SerializeField] private List<ProbabilityLevel> probabilityNode = new List<ProbabilityLevel>();

    // 기존 메서드는 그대로 유지
    private float totalWeight;

    private void SumWeight(int level)
    {
        totalWeight = 0;
        for (int i = 0; i < probabilityNode[level].nodes.Count; i++)
        {
            totalWeight += probabilityNode[level].nodes[i].probability;
        }
    }

    public List<float> GetDrawHeroCardProbability(int level)
    {
        SumWeight(level);

        List<float> heroRarityProbabilityList = new List<float>();

        for (int i = 0; i < probabilityNode[level].nodes.Count; i++)
        {
            heroRarityProbabilityList.Add(probabilityNode[level].nodes[i].probability / totalWeight);
        }

        return heroRarityProbabilityList;
    }

    public HeroRarity GetRarity(int level)
    {
        SumWeight(level);

        float random = Random.Range(0, totalWeight);

        for (int i = 0; i < probabilityNode[level].nodes.Count; i++)
        {
            if (random <= probabilityNode[level].nodes[i].probability)
            {
                return probabilityNode[level].nodes[i].rarity;
            }
            else
            {
                random -= probabilityNode[level].nodes[i].probability;
            }
        }

        return HeroRarity.Normal;
    }
}

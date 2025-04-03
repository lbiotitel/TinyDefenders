using NUnit.Framework;
using System.Collections.Generic;
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
    private float totalWeight;

    private void OnEnable()
    {
        SumWeight();
    }

    private void SumWeight()
    {
        totalWeight = 0;
        for (int i = 0; i < probabilityNode.Length; i++)
        {
            totalWeight += probabilityNode[i].probability;
        }
    }

    public void AddProbability(int level)
    {
        for (int i = 0; i < probabilityNode.Length; i++)
        {
            probabilityNode[i].probability += (float)(level / (i + 1));
        }
    }

    public List<float> GetDrawHeroCardProbability()
    {
        SumWeight();

        List <float> heroRarityProbabilityList = new List<float>();

        for (int i = 0; i < probabilityNode.Length; i++)
        {
            Debug.Log(probabilityNode[i].probability / totalWeight);
            heroRarityProbabilityList.Add(probabilityNode[i].probability / totalWeight);
        }

        return heroRarityProbabilityList;
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

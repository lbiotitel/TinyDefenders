using UnityEngine;

[CreateAssetMenu(fileName = "WaveScriptable", menuName = "Scriptable Objects/WaveScriptable")]
public class WaveScriptable : ScriptableObject
{
    public int wave;
    public float waveTime = 0f;
    public int waveMonsterCount = 0;
    public float spawnInterval = 1f;
    public int waveClearGold;
    public bool bossWave = false;
    public MonsterScriptable spawnMonsterData;

    private void OnValidate()
    {
        if (waveMonsterCount != 0 && waveTime != 0)
        {
            spawnInterval = waveTime / waveMonsterCount;
        }
        wave = int.Parse(name);
    }
}

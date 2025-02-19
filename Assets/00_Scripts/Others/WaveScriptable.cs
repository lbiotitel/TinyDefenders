using UnityEngine;

[CreateAssetMenu(fileName = "WaveScriptable", menuName = "Scriptable Objects/WaveScriptable")]
public class WaveScriptable : ScriptableObject
{
    public int wave;
    public float waveTime;
    public int waveMonsterCount = 0;
    public float spawnInterval = 1f;
    public int waveClearGold;
    public bool bossWave = false;
    public MonsterScriptable spawnMonsterData;
}

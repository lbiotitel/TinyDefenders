using UnityEngine;

public class GameFinishStatGameObject : MonoBehaviour
{

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public string playerName;
    public string friendName;
    public double playerDamage;
    public double friendDamage;
    public int bestWave;
}

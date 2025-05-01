using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileSO", menuName = "Scriptable Objects/ProjectileSO")]
public class ProjectileSO : ScriptableObject
{
    public float Speed;
    public float LifeTime;
    public GameObject DestroyParticle;
    public ProjectileType ProjectileType = ProjectileType.NONE;
    public ProjectileMoveType ProjectileMoveType = ProjectileMoveType.NONE;
    public ProjectileDamageType ProjectileDamageType = ProjectileDamageType.NONE;
    public string ProjectileSFX;
    public float Range;
}


using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileSO", menuName = "Scriptable Objects/ProjectileSO")]
public class ProjectileSO : ScriptableObject
{
    [Header("Base Projectile")]
    public float Speed;
    public float LifeTime;
    public GameObject DestroyParticle;
    public ProjectileType ProjectileType = ProjectileType.NONE;
    public ProjectileMoveType ProjectileMoveType = ProjectileMoveType.NONE;
    public ProjectileDamageType ProjectileDamageType = ProjectileDamageType.NONE;
    public string ProjectileSFX;

    [Header("Range Projectile")]
    public float Range;
    public int TargetCount;
}


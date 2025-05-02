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

    [Header("Range Projectile")]
    public ProjectileRangeSize ProjectileRangeSize = ProjectileRangeSize.NONE;

    [Header("Persistence Projectile")]
    public float damageInterval;

    private void OnValidate ()
    {
        name = string.Format("{0}_{1}_{2}", ProjectileType, ProjectileMoveType, ProjectileDamageType);
        if(ProjectileRangeSize != ProjectileRangeSize.NONE)
            name += string.Format("_{0}", ProjectileRangeSize);
    }
}


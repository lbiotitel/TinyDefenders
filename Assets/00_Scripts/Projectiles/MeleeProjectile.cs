using System.Collections.Generic;
using UnityEngine;

public class MeleeProjectile : BaseProjectile
{
    [SerializeField] private bool currentTransformParticle = false;

    public override void Init(Transform target, Hero referenceHero,double damage, List<DebuffEffect> debuffEffects = null)
    {
        base.Init(target, referenceHero,damage, debuffEffects);
        transform.position = currentTransformParticle ?  transform.position:
            target.transform.position;
        transform.Rotate(0, 90, 0);
        HitMonster();
    }

    protected override void ProjectileLogic() { }

}

using System.Collections.Generic;

public class DamageData
{
    public Hero Hero;
    public Monster Target;
    public double Damage;
    public List<DebuffEffect> DebuffEffects = new List<DebuffEffect>();
}
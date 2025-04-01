public class UndeadSynergy : HeroSynergyBase
{
    public override void HeroSynergyActive(int level)
    {
        if (level < 1) return;
    }

    public override void HeroSynergyDeactive(int level)
    {
        if (level < 1) return;
    }



}

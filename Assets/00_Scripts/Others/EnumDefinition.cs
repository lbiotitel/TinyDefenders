using UnityEngine;


public enum CardType{
    SUMMON,
    ATTACK,
    NOATTACK
}

public struct LayerName {
    public static string Monster = "Monster";
    public static string Hero = "Hero";
}

public enum DebuffType { 
    SLOW,
    STUN,
    BLEED
}

public enum AlarmType { 
    NORMAL,
    GOOD,
    BAD
}

public enum MonsterType { 
    NORMAL,
    BOSS,
    GOLD
}

public enum HostType { 
    HOST,
    CLIENT,
    ALL
}

public enum Localize
{
    UI,
    Hero
}
public enum ProjectileType { 
    NORMAL,
    ROTATE,
}

public enum CardRarity {
    Normal,
    Uncommon,
    Rare
}

public enum HeroRarity { 
    Normal,
    Uncommon,
    Rare,
    Legend,
    God
}

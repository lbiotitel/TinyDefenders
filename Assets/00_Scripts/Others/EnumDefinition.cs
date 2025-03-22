using UnityEngine;


public enum CardType{
    SUMMON,
    ATTACK,
    UTILITY
}

public struct LayerName {
    public static string Monster = "Monster";
    public static string Hero = "Hero";
    public static string Defulat = "Default";
}

public enum HeroTribe { 
    HUMAN,
    DWARF,
    UNDEAD,
    DEVILL,
    ELF,
    ORC,
    SCAVENGER
}

public enum DebuffType { 
    SLOW,
    STUN,
    BLEED,
    POISION,
    FREEZE,
    CHEEL,
}

public enum AlarmType { 
    NORMAL,
    GOOD,
    BAD,
    WARNING
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

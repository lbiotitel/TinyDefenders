using UnityEngine;


public enum CardType{
    SUMMON,
    ATTACK,
    UTILITY
}

public struct SceneName {
    public static string MainScene = "MainScene";
    public static string GameScene = "GamePlayScene";
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
    DEMON,
    ELF,
    ORC,
    SCAVENGER,
    LAST
}

public enum DebuffType { 
    SLOW,
    STUN,
    BLEED,
    POISION,
    FREEZE,
    CHEEL,
    IGNITE
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
    Hero,
    HeroDesc
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
    Legend
}

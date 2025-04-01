using UnityEngine;


public enum CardType{
    SUMMON,
    ATTACK,
    UTILITY
}

public struct SceneName {
    public static string MainScene = "MainScene";
    public static string GameScene = "GamePlayScene";
    public static string IntroScene = "IntroScene";
    public static string ResultScene = "ResultScene";
}

public struct LayerName {
    public static string Monster = "Monster";
    public static string Hero = "Hero";
    public static string Defulat = "Default";
}

public enum LobbyOptionKey { 
    RelayJoinCode,
}

public enum HeroTribe { 
    HUMAN,
    DWARF,
    UNDEAD,
    DEMON,
    ELF,
    ORC,
    SCAVENGER,
    FIVEHERO,
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

public enum PlayerNetworkType { 
    HOST,
    CLIENT,
    ALL
}

public enum NetworkObjectType { 
    HOST,
    CLIENT
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
    Rare,
    Legend
}

public enum HeroRarity { 
    Normal,
    Uncommon,
    Rare,
    Legend
}

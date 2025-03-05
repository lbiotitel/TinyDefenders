using UnityEngine;



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

public enum ProjectileType { 
    NORMAL,
    ROTATE,
}
public enum Rarity { 
    Normal,
    Uncommon,
    Unique,
    Legend
}

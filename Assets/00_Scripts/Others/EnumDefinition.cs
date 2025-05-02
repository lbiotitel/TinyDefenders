using UnityEngine;

public enum MapType { 
    FOREST,
    DESERT,
    CAVE,
    LAVA
}

public enum NetworkMessageType
{
    HOST_SHUT_DOWN,
    CLIENT_DISCONNECT,
}

public enum RANK
{
    BRONZE,
    SILVER,
    GOLD,
    PLATINUM,
    DIAMOND
}

public enum CardType{
    SUMMON,
    ATTACK,
    UTILITY
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
    VILLAGER,
    LAST
}

public enum HeroClass { 
    WARRIOR,
    MAGE,
    ROGUE,
    TANKER,
    ARCHER,
    HUNTER,
    RIDER,
    SPEAR,
    SUPPORTER,
    LAST
}

public enum SynergyType { 
    TRIBE,
    CLASS
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
    WARNING,
    QUEST
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
    HeroDesc,
    Synergy
}
public enum ProjectileType {
    NONE,
    LOOKAT,
    ROTATE,
}

public enum ProjectileDamageType
{
    NONE,
    SINGLE,
    RANGE,
    PERSISTENCE,
}

public enum ProjectileMoveType
{
    NONE,
    STRAIGHT,
    BAZIER,
    MELEE,
    LASER,
    SPIN
}

public enum ProjectileRangeSize
{ 
    NONE,
    SMALL = 1,
    MEDIUM = 2,
    BIG = 3
}

public enum CardRarity {
    Normal,
    Uncommon,
    Rare,
    Legend
}

public enum RelicRarity
{
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

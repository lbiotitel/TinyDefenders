

public struct LobbyInfo
{
    public static string LobbyName = "Tiny Defence";
}


public struct EncryptionType
{
    public static string dEncryption = "dtls";
    public static string wEncryption = "wss";
}

public struct SceneName
{
    public static string MainScene = "MainScene";
    public static string ForestScene = "Forest";
    public static string DesertScene = "Desert";
    public static string CaveScene = "Cave";
    public static string LavaScene = "Lava";
    public static string IntroScene = "IntroScene";
    public static string ResultScene = "ResultScene";
    public static string LoadingScene = "LoadingScene";

    public static string GetMapName(MapType mapType)
    {
        switch (mapType)
        {
            case MapType.FOREST:
                return ForestScene;
            case MapType.DESERT:
                return DesertScene;
            case MapType.CAVE:
                return CaveScene;
            case MapType.LAVA:
                return LavaScene;
        }

        return null;
    }
}

public struct LayerName
{
    public static string Monster = "Monster";
    public static string Hero = "Hero";
    public static string Defulat = "Default";
}

public struct Path { 
    public static string inGameProbabilitySettingPath = "09SettingData/InGameProbabilitySettingData";
    public static string inGameRequireExpPath = "09SettingData/IngameRequireExpSO";

    public static string heroSynergyDataPath = "03HeroSynergyDatas";
    public static string questDataPath = "07QuestDatas";
    public static string userExpDataPath = "09SettingData/UserExpData";
    public static string relicDataPath = "08RelicDatas";
    public static string gameSettingDataPath = "09SettingData/GameSettingSO";   

    public static string wavePath = "06WaveDatas/";
    public static string monsterPath = "05MonsterDatas/";

}

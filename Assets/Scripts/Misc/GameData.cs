using System;

[Serializable]
public class GameData
{
    public TankData[] tankData;
    public int cash;
    public int selectedTankID;
    public bool tutorialCompleted;
    public bool cutSceneCompleted;
    public DateTime lastLaunch;
    public bool adWatchedToday;
    public bool reviewSuggestedToday;
}

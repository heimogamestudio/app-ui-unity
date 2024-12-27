using System;


namespace UIToolkitDemo
{
    /// <summary>
    /// Public static delegates associated with the SettingsScreen/SettingsScreenController.
    ///
    /// Note: these are "events" in the conceptual sense and not the strict C# sense.
    /// </summary>
    public static class SettingsEvents 
    {
        public static Action PlayerFundsReset;
        public static Action PlayerLevelReset;

        public static Action SettingsShown;



        public static Action<string> ThemeSelected;

        // Sync previously saved data from SettingsScreenController to SettingsScreen UI
        public static Action<GameData> GameDataLoaded;

        // Pass updated copy of the game data from the UI to the Controller
        public static Action<GameData> UIGameDataUpdated;

        // Send updated data from the controller to listeners (e.g. GameDataManager, AudioManager, etc.)
        public static Action<GameData> SettingsUpdated;

        public static Action<bool> FpsCounterToggled;

        public static Action<int> TargetFrameRateSet;
    }
}

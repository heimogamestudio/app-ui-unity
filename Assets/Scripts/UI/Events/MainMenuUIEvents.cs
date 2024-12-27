using System;

namespace UIToolkitDemo
{

    /// <summary>
    /// Public static delegates to manage MainMenu UI changes.
    ///
    ///
    /// Note: these are "events" in the conceptual sense and not the strict C# sense.
    /// </summary>
    public static class MainMenuUIEvents
    {

        //Show the HomeScreen to play the game
        public static Action HomeScreenShown;

        //// Show the CharScreen to select characters and gears
        public static Action CharScreenShown;

        // Show the Info Screen with resource links
        public static Action InfoScreenShown;

        // Show the ShopScreen to buy gold/gems/potions
        public static Action ShopScreenShown;

        // Show the ShopScreen but from the OptionsBar
        public static Action OptionsBarShopScreenShown;

        // Show the MailScreen
        public static Action MailScreenShown;

        // Show the SettingsScreen overlay
        public static Action SettingsScreenShown;

        // Show the InventoryScreen
        public static Action InventoryScreenShown;

        public static Action SettingsScreenHidden;

        public static Action InventoryScreenHidden;

        // Show the GameScreen for gameplay
        public static Action GameScreenShown;

        // Triggered when showing a new MenuScreen
        public static Action<MenuScreen> CurrentScreenChanged;

        public static Action<string> CurrentViewChanged;

        // Notifed a TabbedMenu to reset/select first tab
        public static Action<string> TabbedUIReset;
    }
}

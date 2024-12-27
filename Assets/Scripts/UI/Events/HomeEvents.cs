using System.Collections.Generic;
using System;

namespace UIToolkitDemo
{
    /// <summary>
    /// Public static delegates associated with the HomeScreen/HomeScreenController.
    ///
    /// Note: these are "events" in the conceptual sense and not the strict C# sense.
    /// </summary>
    public static class HomeEvents 
    {
        // Triggered to display a welcome message when the HomeScreen appears
        public static Action<string> HomeMessageShown;

        // Event for showing level information
        public static Action<LevelSO> LevelInfoShown;

        // Event for updating/displaying chat window content
        public static Action<List<ChatSO>> ChatWindowShown;

        // Event triggered upon exiting the main menu
        public static Action MainMenuExited;

        // Event triggered when the play button is clicked
        public static Action PlayButtonClicked;

    }
}

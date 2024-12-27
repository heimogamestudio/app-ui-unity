using System;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    public static class GameplayEvents 
    {
        // Triggered after the battle is won
        public static Action WinScreenShown;

        // Triggered after the battle is lost
        public static Action LoseScreenShown;

        // Triggered when a character dies
        public static Action<UnitController> CharacterCardHidden;

        public static Action<GameData> SettingsUpdated;

        // Use the gameData to load the music and sfx volume levels
        public static Action SettingsLoaded;

        // Notify listeners to pause after delay in seconds
        public static Action<float> GamePaused;

        // Resume the game from the PauseScreen
        public static Action GameResumed;

        // Quit the game from the PauseScreen
        public static Action GameQuit;

        // Restart the game from the PauseScreen
        public static Action GameRestarted;

        // Adjust the music volume during gameplay
        public static Action<float> MusicVolumeChanged;

        // Adjust the sound effects volume during gameplay
        public static Action<float> SfxVolumeChanged;

        // Drop a healing potion onto a specific healing slot VisualElement
        public static Action<VisualElement> SlotHealed;

        // Update the number of healing potions
        public static Action<int> HealingPotionUpdated;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace UIToolkitDemo
{
    /// <summary>
    /// Public static delegates associated with the CharScreen/CharScreenController
    ///
    /// Note: these are "events" in the conceptual sense and not the strict C# sense.
    /// </summary>
    public static class CharEvents
    {
        // Update next/last character in the CharScreen
        public static Action NextCharacterSelected;
        public static Action LastCharacterSelected;

        // Triggered when starting up the CharScreen
        public static Action ScreenStarted;

        // Triggered when hiding the CharScreen
        public static Action ScreenEnded;

        // Open the Inventory using a specific gear item slot at index
        public static Action<int> InventoryOpened;

        public static Action GearAutoEquipped;
        public static Action GearAllUnequipped;

        // Level Up Button has been clicked
        public static Action LevelUpClicked;

        // Show the Level up button on the CharScreen
        public static Action<bool> LevelUpButtonEnabled;

        // Level up process has succeeded/failed
        public static Action<bool> CharacterLeveledUp;

        // Level up the character stats
        public static Action<CharacterData> LevelIncremented;

        // Update the Level Meter Window
        public static Action<float> LevelUpdated;

        // Triggered after character previews are initialized.
        public static Action PreviewInitialized;

        // Triggered to display a character
        public static Action<CharacterData> CharacterShown;

        // Triggered when gear item is unequipped
        public static Action<EquipmentSO> GearItemUnequipped;

        // Triggered to update gear slot; provides gear data and slot index.
        public static Action<EquipmentSO, int> GearSlotUpdated;

        // Triggered to auto-equip gear on the current character.
        // Provides the data of the character to be auto-equipped.
        public static Action<CharacterData> CharacterAutoEquipped;

        // Triggered when initializing the starting gear for all characters.
        // Provides the list of characters with their starting gear data.
        public static Action<List<CharacterData>> GearDataInitialized;

        // Triggered when level-up potion is used and provides character data consuming
        // the potion
        public static Action<CharacterData> LevelPotionUsed;
        
        // 
        // Static function to provide LevelMeterData
        public static Func<LevelMeterData> GetLevelMeterData;


    }
}

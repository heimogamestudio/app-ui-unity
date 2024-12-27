using System;
using System.Collections.Generic;

namespace UIToolkitDemo
{
    /// <summary>
    /// Public static delegates associated with the InventoryScreen/InventoryController.
    ///
    /// Note: these are "events" in the conceptual sense and not the strict C# sense.
    /// </summary>
    public static class InventoryEvents 
    {
        // Event triggered when a gear item is clicked
        public static Action<GearItemComponent> GearItemClicked;

        // Event triggered when the inventory screen appears
        public static Action ScreenEnabled;

        // Event triggered when selecting a gear item
        public static Action<EquipmentSO> GearSelected;

        // Event for updating the filtered gear items
        public static Action<Rarity, EquipmentType> GearFiltered;

        // Event for initial setup
        public static Action InventorySetup;

        // Event when refreshing the inventory
        public static Action<List<EquipmentSO>> InventoryUpdated;

        // Event for auto-equipping from Character Screen
        public static Action<List<EquipmentSO>> GearAutoEquipped;
    }
}

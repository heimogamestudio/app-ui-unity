using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIToolkitDemo
{
    /// <summary>
    /// Public static delegates associated with the ShopScreen/ShopScreenController
    ///
    /// Note: these are "events" in the conceptual sense and not the strict C# sense.
    /// </summary>
    public static class ShopEvents 
    {
        // Select the Gold/Gem/Potions tabs in the ShopScreen
        public static Action GoldSelected;
        public static Action GemSelected;
        public static Action PotionSelected;

        // Triggered when selecting a tab by name
        public static Action<string> TabSelected;

        // Click the Buy button on a ShopItemComponent (pass ShopItem data with screen click position)
        public static Action<ShopItemSO, Vector2> ShopItemClicked;

        // Notify the ShopScreenController
        public static Action<ShopItemSO, Vector2> ShopItemPurchasing;

        public static Action<List<ShopItemSO>> ShopUpdated;

        // Update the GameData when consuming potions
        public static Action<GameData> PotionsUpdated;

        // Update the OptionsBar when buying an itme
        public static Action<GameData> FundsUpdated;

        // Process a free gift in a mail message
        public static Action<ShopItemType, uint, Vector2> RewardProcessed;

        // Successful purchase of an item
        public static Action<ShopItemSO, Vector2> TransactionProcessed;

        // Failed purchase on insufficient funds
        public static Action<ShopItemSO> TransactionFailed;
    }
}

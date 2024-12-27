using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.Localization;

namespace UIToolkitDemo
{
    /// <summary>
    /// This is a non-modal overlay screen, not managed by the UIManager. This can appear to notify the user with temporary
    /// text notifications.
    /// </summary>
    public class PopUpText : MenuScreen
    {
        // Rich text color highlight
        public static readonly string TextHighlight = "F8BB19";

        // Contains basic text styling
        const string k_PopUpTextClass = "popup-text";

        // Each message contains its own styles

        // ShopScreen message classes
        const string k_ShopActiveClass = "popup-text-active";
        const string k_ShopInactiveClass = "popup-text-inactive";

        // CharScreen message classes
        const string k_GearActiveClass = "popup-text-active--left";
        const string k_GearInactiveClass = "popup-text-inactive--left";

        // HomeScreen message classes
        const string k_HomeActiveClass = "popup-text-active--home";
        const string k_HomeInactiveClass = "popup-text-inactive--home";

        // Mail message reward classes
        const string k_MailActiveClass = "popup-text-active--reward";
        const string k_MailInactiveClass = "popup-text-inactive--reward";

        // Delay between welcome messages
        const float k_HomeMessageCooldown = 15f;

        float m_TimeToNextHomeMessage = 0f;
        Label m_PopUpText;

        // Customizes active/inactive styles, duration, and delay for each text prompt
        float m_Delay = 0f;
        float m_Duration = 1f;
        string m_ActiveClass;
        string m_InactiveClass;
        
        // Check for portrait/landscape changes
        MediaQuery m_MediaQuery;
        MediaAspectRatio m_MediaAspectRatio;
        
        // Localized strings with cached text
        readonly LocalizedString m_InsufficientFundsLocalized = new LocalizedString 
        { 
            TableReference = "SettingsTable",
            TableEntryReference = "PopUp.InsufficientFunds"
        };
        string m_LocalizedInsufficientFunds;
    
        readonly LocalizedString m_GearEquippedLocalized = new LocalizedString 
        { 
            TableReference = "SettingsTable",
            TableEntryReference = "PopUp.GearEquipped"
        };
        string m_LocalizedGearEquipped;
    
        readonly LocalizedString m_InsufficientPotionsLocalized = new LocalizedString 
        { 
            TableReference = "SettingsTable",
            TableEntryReference = "PopUp.InsufficientPotions"
        };
        string m_LocalizedInsufficientPotions;
    
        readonly LocalizedString m_ItemAddedLocalized = new LocalizedString 
        { 
            TableReference = "SettingsTable",
            TableEntryReference = "PopUp.ItemAddedToInventory"
        };
        string m_LocalizedItemAdded;
    
        readonly LocalizedString m_PotionsAddedLocalized = new LocalizedString 
        { 
            TableReference = "SettingsTable",
            TableEntryReference = "PopUp.PotionsAddedToInventory"
        };
        string m_LocalizedPotionsAdded;

        // Store current message parameters
        ShopItemSO m_CurrentShopItem;
        string m_CurrentGearName;
        (uint quantity, ShopItemType type) m_CurrentReward;

        // Properties to format the text
        string GetInsufficientFundsText => string.Format(m_LocalizedInsufficientFunds, 
            m_CurrentShopItem.ItemName, 
            m_CurrentShopItem.CostInCurrencyType);

        string GetGearEquippedText => string.Format(m_LocalizedGearEquipped, 
            m_CurrentGearName);

        string GetItemAddedText => string.Format(m_LocalizedItemAdded, 
            m_CurrentShopItem.ItemName);

        string GetPotionsAddedText => string.Format(m_LocalizedPotionsAdded, 
            m_CurrentReward.quantity, 
            m_CurrentReward.type,
            m_CurrentReward.quantity > 1 ? "s" : string.Empty);

        void OnEnable()
        {
            InventoryEvents.GearSelected += OnGearSelected;

            ShopEvents.TransactionProcessed += OnShopTransactionProcessed;
            ShopEvents.RewardProcessed += OnMailRewardProcessed;
            ShopEvents.TransactionFailed += OnTransactionFailed;

            CharEvents.CharacterLeveledUp += OnCharacterLeveledUp;

            HomeEvents.HomeMessageShown += OnHomeMessageShown;
            
            // Localization event handlers
            m_InsufficientFundsLocalized.StringChanged += OnInsufficientFundsChanged;
            m_GearEquippedLocalized.StringChanged += OnGearEquippedChanged;
            m_InsufficientPotionsLocalized.StringChanged += OnInsufficientPotionsChanged;
            m_ItemAddedLocalized.StringChanged += OnItemAddedChanged;
            m_PotionsAddedLocalized.StringChanged += OnPotionsAddedChanged;
        }

        void OnDisable()
        {
            InventoryEvents.GearSelected -= OnGearSelected;

            ShopEvents.TransactionProcessed -= OnShopTransactionProcessed;
            ShopEvents.RewardProcessed -= OnMailRewardProcessed;
            ShopEvents.TransactionFailed -= OnTransactionFailed;

            CharEvents.CharacterLeveledUp -= OnCharacterLeveledUp;

            HomeEvents.HomeMessageShown -= OnHomeMessageShown;
            
            // Unsubscribe
            m_InsufficientFundsLocalized.StringChanged -= OnInsufficientFundsChanged;
            m_GearEquippedLocalized.StringChanged -= OnGearEquippedChanged;
            m_InsufficientPotionsLocalized.StringChanged -= OnInsufficientPotionsChanged;
            m_ItemAddedLocalized.StringChanged -= OnItemAddedChanged;
            m_PotionsAddedLocalized.StringChanged -= OnPotionsAddedChanged;
        }


        protected override void Awake()
        {
            base.Awake();
            SetVisualElements();

            m_ActiveClass = k_ShopActiveClass;
            m_InactiveClass = k_ShopInactiveClass;

            SetupText();
            HideText();
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_PopUpText = m_Root.Q<Label>("main-menu-popup_text");

            if (m_PopUpText != null)
            {
                m_PopUpText.text = string.Empty;
            }
        }

        void ShowMessage(string message)
        {
            if (m_PopUpText == null || string.IsNullOrEmpty(message))
            {
                return;
            }

            StartCoroutine(ShowMessageRoutine(message));
        }

        IEnumerator ShowMessageRoutine(string message)
        {
            if (m_PopUpText != null)
            {
                m_PopUpText.text = message;

                // reset any leftover Selectors
                SetupText();

                // hide text
                HideText();

                // wait for delay
                yield return new WaitForSeconds(m_Delay);

                // show text and wait for duration
                ShowText();
                yield return new WaitForSeconds(m_Duration);

                // hide text again
                HideText();
            }
        }

        void HideText()
        {
            m_PopUpText.RemoveFromClassList(m_ActiveClass);
            m_PopUpText.AddToClassList(m_InactiveClass);
        }

        void ShowText()
        {
            m_PopUpText.RemoveFromClassList(m_InactiveClass);
            m_PopUpText.AddToClassList(m_ActiveClass);
        }

        // clear any remaining Selectors and add base styling 
        void SetupText()
        {
            m_PopUpText.ClearClassList();
            m_PopUpText.AddToClassList(k_PopUpTextClass);
        }

        // event-handling methods

        // Localization event handlers
        void OnInsufficientFundsChanged(string localizedText) => m_LocalizedInsufficientFunds = localizedText;
        void OnGearEquippedChanged(string localizedText) => m_LocalizedGearEquipped = localizedText;
        void OnInsufficientPotionsChanged(string localizedText) => m_LocalizedInsufficientPotions = localizedText;
        void OnItemAddedChanged(string localizedText) => m_LocalizedItemAdded = localizedText;
        void OnPotionsAddedChanged(string localizedText) => m_LocalizedPotionsAdded = localizedText;

        
        void OnTransactionFailed(ShopItemSO shopItemSO)
        {
            // use a longer delay, messages are longer here
            m_Delay = 0f;
            m_Duration = 1.2f;

            // centered on ShopScreen
            m_ActiveClass = k_ShopActiveClass;
            m_InactiveClass = k_ShopInactiveClass;

            m_CurrentShopItem = shopItemSO;
            ShowMessage(GetInsufficientFundsText);
            m_CurrentShopItem = null;
        }

        void OnGearSelected(EquipmentSO gear)
        {
            m_Delay = 0f;
            m_Duration = 0.8f;

            // centered on CharScreen
            m_ActiveClass = k_GearActiveClass;
            m_InactiveClass = k_GearInactiveClass;

            m_CurrentGearName = gear.equipmentName;
            ShowMessage(GetGearEquippedText);
            m_CurrentGearName = null;
        }

        void OnCharacterLeveledUp(bool state)
        {
            // Only show text warning on failure
            if (!state && m_PopUpText != null)
            {
                m_Delay = 0f;
                m_Duration = 0.8f;
                m_ActiveClass = k_GearActiveClass;
                m_InactiveClass = k_GearInactiveClass;

                ShowMessage(m_LocalizedInsufficientPotions);
            }
        }

        void OnHomeMessageShown(string message)
        {
            // welcome the player when on the HomeScreen
            if (Time.time >= m_TimeToNextHomeMessage)
            {
                // timing and position
                m_Delay = 0.25f;
                m_Duration = 1.5f;

                // centered below title
                m_ActiveClass = k_HomeActiveClass;
                m_InactiveClass = k_HomeInactiveClass;

                ShowMessage(message);

                // cooldown delay to avoid spamming
                m_TimeToNextHomeMessage = Time.time + k_HomeMessageCooldown;
            }
        }

        void OnShopTransactionProcessed(ShopItemSO shopItem, Vector2 screenPos)
        {
            // show message when purchasing potions (not gold or gems)
            if (shopItem.ContentType == ShopItemType.LevelUpPotion || shopItem.ContentType == ShopItemType.HealthPotion)
            {

                // timing and position
                m_Delay = 0f;
                m_Duration = 0.8f;

                // centered on ShopScreen
                m_ActiveClass = k_ShopActiveClass;
                m_InactiveClass = k_ShopInactiveClass;

                m_CurrentShopItem = shopItem;
                ShowMessage(GetItemAddedText);
                m_CurrentShopItem = null;
            }
        }

        void OnMailRewardProcessed(ShopItemType rewardType, uint rewardQuantity, Vector2 screenPos)
        {
            // show message when purchasing potions (not gold or gems)
            if (rewardType == ShopItemType.LevelUpPotion || rewardType == ShopItemType.HealthPotion)
            {

                // timing and position
                m_Delay = 0f;
                m_Duration = 1.2f;
                
                // centered on right Mail Content Screen
                m_ActiveClass = k_MailActiveClass;
                m_InactiveClass = k_MailInactiveClass;

                m_CurrentReward = (rewardQuantity, rewardType);
                ShowMessage(GetPotionsAddedText);
                m_CurrentReward = default;
            }
        }
    }
}

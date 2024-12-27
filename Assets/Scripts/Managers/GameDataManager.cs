using UnityEngine;
using System;
using UnityEngine.Localization;

namespace UIToolkitDemo
{
    [RequireComponent(typeof(SaveManager))]
    public class GameDataManager : MonoBehaviour
    {

        public static Action GameDataRequested;
        public static event Action<GameData> GameDataReceived;
        
        [SerializeField] GameData m_GameData;
        public GameData GameData { set => m_GameData = value; get => m_GameData; }

        SaveManager m_SaveManager;
        bool m_IsGameDataInitialized;
        
        LocalizedString m_WelcomeMessageLocalized = new LocalizedString 
        { 
            TableReference = "SettingsTable",
            TableEntryReference = "PopUp.WelcomeMessage"
        };
        string m_LocalizedWelcomeMessage;

        void OnEnable()
        {
            MainMenuUIEvents.HomeScreenShown += OnHomeScreenShown;

            CharEvents.CharacterShown += OnCharacterShown;
            CharEvents.LevelPotionUsed += OnLevelPotionUsed;

            SettingsEvents.SettingsUpdated += OnSettingsUpdated;
            SettingsEvents.PlayerFundsReset += OnResetFunds;

            ShopEvents.ShopItemPurchasing += OnPurchaseItem;

            MailEvents.RewardClaimed += OnRewardClaimed;

            // Listen for requests
            GameDataRequested += OnGameDataRequested;
            
            m_WelcomeMessageLocalized.StringChanged += OnWelcomeMessageChanged;
        }

        void OnDisable()
        {
            MainMenuUIEvents.HomeScreenShown -= OnHomeScreenShown;

            CharEvents.CharacterShown -= OnCharacterShown;
            CharEvents.LevelPotionUsed -= OnLevelPotionUsed;

            SettingsEvents.SettingsUpdated -= OnSettingsUpdated;
            SettingsEvents.PlayerFundsReset -= OnResetFunds;

            ShopEvents.ShopItemPurchasing -= OnPurchaseItem;

            MailEvents.RewardClaimed -= OnRewardClaimed;
            
            GameDataRequested -= OnGameDataRequested;
            
            m_WelcomeMessageLocalized.StringChanged -= OnWelcomeMessageChanged;
        }

        void Awake()
        {
            m_SaveManager = GetComponent<SaveManager>();
        }

        void Start()
        {
            //if saved data exists, load saved data
            m_SaveManager.LoadGame();

            // flag that GameData is loaded the first time
            m_IsGameDataInitialized = true;

            // Force load the localized string
            m_LocalizedWelcomeMessage = m_WelcomeMessageLocalized.GetLocalizedString();
            ShowWelcomeMessage();

            UpdateFunds();
            UpdatePotions();
        }

        // transaction methods 
        void UpdateFunds()
        {
            if (m_GameData != null)
                ShopEvents.FundsUpdated?.Invoke(m_GameData);
        }

        void UpdatePotions()
        {
            if (m_GameData != null)
                ShopEvents.PotionsUpdated?.Invoke(m_GameData);
        }

        bool HasSufficientFunds(ShopItemSO shopItem)
        {
            if (shopItem == null)
                return false;

            CurrencyType currencyType = shopItem.CostInCurrencyType;

            float discountedPrice = (((100 - shopItem.Discount) / 100f) * shopItem.Cost);

            switch (currencyType)
            {
                case CurrencyType.Gold:
                    return m_GameData.Gold >= discountedPrice;

                case CurrencyType.Gems:
                    return m_GameData.Gems >= discountedPrice;

                case CurrencyType.USD:
                    return true;

                default:
                    return false;
            }
        }

        // do we have enough potions to level up?
        public bool CanLevelUp(CharacterData character)
        {
            if (m_GameData == null || character == null)
                return false;

            return (character.GetPotionsForNextLevel() <= m_GameData.LevelUpPotions);
        }

        void PayTransaction(ShopItemSO shopItem)
        {
            if (shopItem == null)
                return;

            CurrencyType currencyType = shopItem.CostInCurrencyType;

            float discountedPrice = (((100 - shopItem.Discount) / 100f) * shopItem.Cost);

            switch (currencyType)
            {
                case CurrencyType.Gold:
                    m_GameData.Gold -= (uint)discountedPrice;
                    break;

                case CurrencyType.Gems:
                    m_GameData.Gems -= (uint)discountedPrice;
                    break;

                // non-monetized placeholder - invoke in-app purchase logic/interface for a real application
                case CurrencyType.USD:
                    break;
            }
        }

        void PayLevelUpPotions(uint numberPotions)
        {
            if (m_GameData != null)
            {
                m_GameData.LevelUpPotions -= numberPotions;

                ShopEvents.PotionsUpdated?.Invoke(m_GameData);
            }
        }

        void ReceivePurchasedGoods(ShopItemSO shopItem)
        {
            if (shopItem == null)
                return;

            ShopItemType contentType = shopItem.ContentType;
            uint contentValue = shopItem.ContentAmount;

            ReceiveContent(contentType, contentValue);
        }

        // for gifts or purchases
        void ReceiveContent(ShopItemType contentType, uint contentValue)
        {
            switch (contentType)
            {
                case ShopItemType.Gold:
                    m_GameData.Gold += contentValue;
                    UpdateFunds();
                    break;

                case ShopItemType.Gems:
                    m_GameData.Gems += contentValue;
                    UpdateFunds();
                    break;

                case ShopItemType.HealthPotion:
                    m_GameData.HealthPotions += contentValue;
                    UpdatePotions();
                    UpdateFunds();
                    break;

                case ShopItemType.LevelUpPotion:
                    m_GameData.LevelUpPotions += contentValue;

                    UpdatePotions();
                    UpdateFunds();
                    break;
            }
        }

        void ShowWelcomeMessage()
        {
            if (string.IsNullOrEmpty(m_LocalizedWelcomeMessage))
            {
                // Fallback in case the string hasn't loaded yet
                m_LocalizedWelcomeMessage = m_WelcomeMessageLocalized.GetLocalizedString();
            }

            string message = string.Format(m_LocalizedWelcomeMessage, GameData.UserName);
            HomeEvents.HomeMessageShown?.Invoke(message);
        }


        // event-handling methods

        void OnWelcomeMessageChanged(string localizedText)
        {
            m_LocalizedWelcomeMessage = localizedText;
        }
        
        // If a UI component asks for GameData, serve it out for data-binding purposes
        void OnGameDataRequested()
        {
            GameDataReceived?.Invoke(m_GameData);
        }
        
        // buying item from ShopScreen, pass button screen position 
        void OnPurchaseItem(ShopItemSO shopItem, Vector2 screenPos)
        {
            if (shopItem == null)
                return;

            // invoke transaction succeeded or failed
            if (HasSufficientFunds(shopItem))
            {
                PayTransaction(shopItem);
                ReceivePurchasedGoods(shopItem);
                ShopEvents.TransactionProcessed?.Invoke(shopItem, screenPos);

                AudioManager.PlayDefaultTransactionSound();
            }
            else
            {
                // notify listeners (PopUpText, sound, etc.)
                ShopEvents.TransactionFailed?.Invoke(shopItem);
                AudioManager.PlayDefaultWarningSound();
            }
        }

        // gift from a Mail Message
        void OnRewardClaimed(MailMessageSO msg, Vector2 screenPos)
        {
            if (msg == null)
                return;

            ShopItemType rewardType = msg.RewardType;

            uint rewardValue = msg.RewardValue;

            ReceiveContent(rewardType, rewardValue);

            ShopEvents.RewardProcessed?.Invoke(rewardType, rewardValue, screenPos);
            AudioManager.PlayDefaultTransactionSound();
        }

        // update values from SettingsScreen
        void OnSettingsUpdated(GameData gameData)
        {

            if (gameData == null)
                return;

            m_GameData.SfxVolume = gameData.SfxVolume;
            m_GameData.MusicVolume = gameData.MusicVolume;
            m_GameData.LanguageSelection = gameData.LanguageSelection;
            m_GameData.IsFpsCounterEnabled = gameData.IsFpsCounterEnabled;
            m_GameData.IsToggled = gameData.IsToggled;
            m_GameData.Theme = gameData.Theme;
            m_GameData.UserName = gameData.UserName;
            m_GameData.TargetFrameRateSelection = gameData.TargetFrameRateSelection;
        }

        // Attempt to level up the character using a potion
        void OnLevelPotionUsed(CharacterData charData)
        {
            if (charData == null)
                return;

            bool isLeveled = false;
            if (CanLevelUp(charData))
            {
                PayLevelUpPotions(charData.GetPotionsForNextLevel());
                isLeveled = true;
                AudioManager.PlayVictorySound();
            }
            else
            {
                AudioManager.PlayDefaultWarningSound();
            }

            // Notify other objects if level up succeeded or failed
            CharEvents.CharacterLeveledUp?.Invoke(isLeveled);
        }

        void OnResetFunds()
        {
            m_GameData.Gold = 0;
            m_GameData.Gems = 0;
            m_GameData.HealthPotions = 0;
            m_GameData.LevelUpPotions = 0;
            UpdateFunds();
            UpdatePotions();
        }

        void OnHomeScreenShown()
        {
            if (m_IsGameDataInitialized)
            {
                ShowWelcomeMessage();
            }
        }

        void OnCharacterShown(CharacterData charData)
        {
            // notify the CharScreen to enable or disable the LevelUpButton VFX
            CharEvents.LevelUpButtonEnabled?.Invoke(CanLevelUp(charData));
        }

    }
}

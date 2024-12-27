using Unity.Properties;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// Represents a type of item in the shop: soft currency (gold), hard currency (gems), health potions, or
    /// level up potions.
    /// </summary>
    [System.Serializable]
    public enum ShopItemType
    {
        Gold, // soft currency (in-game)
        Gems, // hard currency (buy with real money)
        HealthPotion, // example of an item used in gameplay (non-functional in this demo)
        LevelUpPotion // levels up the character (PowerPotion)
    }

    /// <summary>
    /// The type of currency used to purchase the item (gold, gems, USD).
    /// </summary>
    [System.Serializable]
    public enum CurrencyType
    {
        Gold,
        Gems,
        USD // use real money to buy gems (free in this demo)
    }

    /// <summary>
    /// Represents an item available for purchase in the game's shop, including all properties such as name,
    /// icon, cost, discount, and content type.
    /// </summary>
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/ShopItems/ShopItemGameData",
        menuName = "UIToolkitDemo/ShopItem", order = 4)]
    public class ShopItemSO : ScriptableObject
    {

        [Tooltip("The name of the item to purchase")]
        [SerializeField] string m_ItemName;

        [SerializeField] LocalizedString m_ItemNameLocalized;

        [Tooltip("Sprite used to represent the product")]
        [SerializeField] Sprite m_Icon;

        [Tooltip("FREE if equal to 0; cost amount in CostInCurrencyType below")]
        [SerializeField] float m_Cost;
        
        [Tooltip("UI shows tag if value larger than 0 (percentage off)")]
        [SerializeField] uint m_Discount;
        
        [Tooltip("if not empty, UI shows a banner with this text")]
        [SerializeField] string m_PromoBannerText;
        [Tooltip("if not empty, UI shows a banner with this text")]
        [SerializeField] LocalizedString m_PromoBannerLocalized;

        [SerializeField] LocalizedString m_FreeTextLocalized = new LocalizedString("SettingsTable", "Shop_Free");
        [CreateProperty] public LocalizedString FreeTextLocalized => m_FreeTextLocalized;
        
        [Tooltip("How many potions/coins this item gives the player upon purchase")]
        [SerializeField] uint m_ContentValue;
        
        [Tooltip("The type of shop item (coins/gems/potions) to purchase")]
        [SerializeField] ShopItemType m_ContentType;
        
        // Properties
        [CreateProperty] public LocalizedString ItemNameLocalized => m_ItemNameLocalized;
        [CreateProperty] public LocalizedString PromoBannerLocalized => m_PromoBannerLocalized;
        
        [CreateProperty] public string ItemName => m_ItemName;
        [CreateProperty] public Sprite Icon => m_Icon;
        [CreateProperty] public float Cost => m_Cost;
        [CreateProperty] public uint Discount => m_Discount;
        [CreateProperty] public string PromoBannerText => m_PromoBannerText;
        [CreateProperty] public uint ContentAmount => m_ContentValue;
        [CreateProperty] public ShopItemType ContentType => m_ContentType;

        // Resource location for sprites/icons
        const string k_ResourcePath = "GameData/GameIcons";
        
        // ScriptableObject pairing icons with currency/shop item types
        GameIconsSO m_GameIconsData;

        void OnEnable()
        {
            m_GameIconsData = Resources.Load<GameIconsSO>(k_ResourcePath);
        }

        /// <summary>
        ///  Determines the currency type required to purchase this item. Soft currency (gold) costs
        /// hard current (gems). Hard currency costs real USD. Health potions costs soft currency (gold).
        /// Level up potions cost hard currency (gems).
        /// </summary>
        [CreateProperty]
        public CurrencyType CostInCurrencyType
        {
            get
            {
                switch (m_ContentType)
                {
                    case (ShopItemType.Gold):
                        return CurrencyType.Gems;
                    case (ShopItemType.Gems):
                        return CurrencyType.USD;
                    case (ShopItemType.HealthPotion):
                        return CurrencyType.Gold;
                    case (ShopItemType.LevelUpPotion):
                        return CurrencyType.Gems;
                    default:
                        return CurrencyType.Gems;
                }
            }
        }        

        [CreateProperty]
        public DisplayStyle BannerDisplayStyle
        {
            get => string.IsNullOrEmpty(m_PromoBannerText) ? DisplayStyle.None : DisplayStyle.Flex;
        }
        
        [CreateProperty]
        public Sprite ContentCurrencyIcon
        {
            get => m_GameIconsData.GetShopTypeIcon(m_ContentType); // Retrieve the appropriate icon based on ContentType
        }

        [CreateProperty]
        public string FormattedContentValue
        {
            get => $" {m_ContentValue}"; // Return the content value as a formatted string with a leading space
        }
        
        [CreateProperty]
        public bool IsFree
        {
            get => Cost <= 0.00001f;
        }

        [CreateProperty]
        public bool IsCostInUSD
        {
            get => CostInCurrencyType == CurrencyType.USD;
        }

        /// <summary>
        /// Formatted cost text.
        /// </summary>
        [CreateProperty]
        public string FormattedCost
        {
            get
            {
                if (Cost <= 0.00001f)
                    return string.Empty;  // We'll use the localized version instead for free items
            
                string currencyPrefix = (CostInCurrencyType == CurrencyType.USD) ? "$" : string.Empty;
                string decimalPlaces = (CostInCurrencyType == CurrencyType.USD) ? "0.00" : "0";
                return currencyPrefix + Cost.ToString(decimalPlaces);
            }
        }

        [CreateProperty]
        public bool UseFreeText => Cost <= 0.00001f;

        [CreateProperty] 
        public DisplayStyle RegularPriceDisplay => UseFreeText ? DisplayStyle.None : DisplayStyle.Flex;

        [CreateProperty] 
        public DisplayStyle FreeTextDisplay => UseFreeText ? DisplayStyle.Flex : DisplayStyle.None;
        
        
        [CreateProperty]
        public Sprite CostIconSprite
        {
            get => m_GameIconsData.GetCurrencyIcon(CostInCurrencyType);
        }

        [CreateProperty]
        public DisplayStyle CostIconGroupDisplayStyle
        {
            get =>  IsFree || IsCostInUSD ? DisplayStyle.None : DisplayStyle.Flex;
        }

        [CreateProperty]
        public string DiscountText
        {
            get => Discount > 0 ? $"{Discount}%" : string.Empty;
        }

        [CreateProperty]
        public string DiscountedCost
        {
            get
            {
                if (Discount > 0)
                {
                    string currencyPrefix = (CostInCurrencyType == CurrencyType.USD) ? "$" : string.Empty;
                    string decimalPlaces = (CostInCurrencyType == CurrencyType.USD) ? "0.00" : "0"; // No decimals if not USD
                    return currencyPrefix + (((100 - Discount) / 100f) * Cost).ToString(decimalPlaces);
                }
                return string.Empty;
            }
        }

        [CreateProperty]
        public DisplayStyle DiscountIconGroupDisplayStyle
        {
            get => CostInCurrencyType == CurrencyType.USD ? DisplayStyle.None : DisplayStyle.Flex;
        }
        
        [CreateProperty]
        public bool IsDiscounted
        {
            get => Discount > 0;
        }

        [CreateProperty]
        public DisplayStyle DiscountDisplayStyle
        {
            get => IsDiscounted ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
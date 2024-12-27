using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UIToolkitDemo
{
    // presenter/controller for the ShopScreen
    public class ShopScreenController : MonoBehaviour
    {

        [Tooltip("Path within the Resources folders for MailMessage ScriptableObjects.")]
        [SerializeField] string m_ResourcePath = "GameData/ShopItems";

        // ScriptableObject game data from Resources
        List<ShopItemSO> m_ShopItems = new List<ShopItemSO>();

        // game data filtered in categories
        List<ShopItemSO> m_GoldShopItems = new List<ShopItemSO>();
        List<ShopItemSO> m_GemShopItems = new List<ShopItemSO>();
        List<ShopItemSO> m_PotionShopItems = new List<ShopItemSO>();

        void OnEnable()
        {
            ShopEvents.ShopItemClicked += OnTryBuyItem;

            ShopEvents.GoldSelected += OnGoldSelected;
            ShopEvents.GemSelected += OnGemSelected;
            ShopEvents.PotionSelected += OnPotionSelected;
        }

        void OnDisable()
        {
            ShopEvents.ShopItemClicked -= OnTryBuyItem;

            ShopEvents.GoldSelected -= OnGoldSelected;
            ShopEvents.GemSelected -= OnGemSelected;
            ShopEvents.PotionSelected -= OnPotionSelected;
        }

        void Start()
        {
            LoadShopData();

            ShopEvents.ShopUpdated?.Invoke(m_GoldShopItems);
        }

        // fill the ShopScreen with data
        void LoadShopData()
        {
            // Load the ScriptableObjects from the Resources directory (default = Resources/GameData/)
            m_ShopItems.AddRange(Resources.LoadAll<ShopItemSO>(m_ResourcePath));

            // sort them by type
            m_GoldShopItems = m_ShopItems.Where(c => c.ContentType == ShopItemType.Gold).ToList();
            m_GemShopItems = m_ShopItems.Where(c => c.ContentType == ShopItemType.Gems).ToList();
            m_PotionShopItems = m_ShopItems.Where(c => c.ContentType == ShopItemType.HealthPotion || c.ContentType == ShopItemType.LevelUpPotion).ToList();

            m_GoldShopItems = SortShopItems(m_GoldShopItems);
            m_GemShopItems = SortShopItems(m_GemShopItems);
            m_PotionShopItems = SortShopItems(m_PotionShopItems);
        }

        List<ShopItemSO> SortShopItems(List<ShopItemSO> originalList)
        {
            return originalList.OrderBy(x => x.Cost).ToList();
        }

        // try to buy the item, pass the screen coordinate of the buy button
        void OnTryBuyItem(ShopItemSO shopItemData, Vector2 screenPos)
        {
            if (shopItemData == null)
                return;

            // notify other objects we are trying to buy an item
            ShopEvents.ShopItemPurchasing?.Invoke(shopItemData, screenPos);
        }

        void OnPotionSelected()
        {
            ShopEvents.ShopUpdated?.Invoke(m_PotionShopItems);
        }

        void OnGemSelected()
        {
            ShopEvents.ShopUpdated?.Invoke(m_GemShopItems);
        }

        void OnGoldSelected()
        {
            ShopEvents.ShopUpdated?.Invoke(m_GoldShopItems);
        }
    }
}
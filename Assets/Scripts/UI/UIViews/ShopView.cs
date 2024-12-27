
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// 
    /// </summary>
    public class ShopView : UIView
    {
        // Class selector for a clickable tab button
        const string k_TabClass = "shoptab";

        // Class selector for currently selected tab button
        const string k_SelectedTabClass = "selected-shoptab";
        const string k_ShopTemplateContainerClass = "shop-item__template-container";

        // Resource location for sprites/icons
        const string k_ResourcePath = "GameData/GameIcons";

        [Header("Shop Item")]
        [Tooltip("ShopItem Element Asset to instantiate ")]
        [SerializeField] VisualTreeAsset m_ShopItemAsset;
        [SerializeField] GameIconsSO m_GameIconsData;

        // Visual elements
        VisualElement m_ShopScrollView;

        VisualElement m_GoldTabButton;
        VisualElement m_GemTabButton;
        VisualElement m_PotionTabButton;

        public ShopView(VisualElement topElement) : base(topElement)
        {
            ShopEvents.ShopUpdated += OnShopUpdated;
            ShopEvents.TabSelected += OnTabSelected;

            // This ScriptableObject pairs data types (ShopItems, Skills, Rarity, Classes, etc.) with specific icons 
            // (default path = Resources/GameData/GameIcons)
            m_GameIconsData = Resources.Load<GameIconsSO>(k_ResourcePath);
            m_ShopItemAsset = Resources.Load<VisualTreeAsset>("ShopItem") as VisualTreeAsset;
        }

        public override void Dispose()
        {
            base.Dispose();
            ShopEvents.ShopUpdated -= OnShopUpdated;
            ShopEvents.TabSelected -= OnTabSelected;

            UnregisterButtonCallbacks();
        }

        private void OnTabSelected(string tabName)
        {
            SelectTab(tabName);
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_ShopScrollView = m_TopElement.Q<VisualElement>("shop__content-scrollview");

            m_GoldTabButton = m_TopElement.Q<VisualElement>("shop-gold-shoptab");
            m_GemTabButton = m_TopElement.Q<VisualElement>("shop-gem-shoptab");
            m_PotionTabButton = m_TopElement.Q<VisualElement>("shop-potion-shoptab");
        }

        // Optional: Unregistering the button callbacks is not strictly necessary
        // in most cases and depends on your application's lifecycle management.
        // You can choose to unregister them if needed for specific scenarios.
        protected void UnregisterButtonCallbacks()
        {
            m_GoldTabButton.UnregisterCallback<ClickEvent>(SelectGoldTab);
            m_GemTabButton.UnregisterCallback<ClickEvent>(SelectGemsTab);
            m_PotionTabButton.UnregisterCallback<ClickEvent>(SelectPotionTab);
        }

        protected override void RegisterButtonCallbacks()
        {
            m_GoldTabButton.RegisterCallback<ClickEvent>(SelectGoldTab);
            m_GemTabButton.RegisterCallback<ClickEvent>(SelectGemsTab);
            m_PotionTabButton.RegisterCallback<ClickEvent>(SelectPotionTab);
        }

        void SelectPotionTab(ClickEvent evt)
        {
            ClickTabButton(evt);
            ShopEvents.PotionSelected?.Invoke();
        }

        void SelectGemsTab(ClickEvent evt)
        {
            ClickTabButton(evt);
            ShopEvents.GemSelected?.Invoke();
        }

        void SelectGoldTab(ClickEvent evt)
        {
            ClickTabButton(evt);
            ShopEvents.GoldSelected?.Invoke();
        }

        // Handle click action for the tab buttons using ClickEvent
        void ClickTabButton(ClickEvent evt)
        {
            VisualElement clickedTab = evt.currentTarget as VisualElement;

            ClickTabButton(clickedTab);
        }

        // Handle click action for the tab buttons using VisualElement
        void ClickTabButton(VisualElement clickedTab)
        {
            // if the clicked tab is not already selected, select it
            if (!IsTabSelected(clickedTab))
            {
                // de-select any other tabs that are currently active
                UnselectOtherTabs(clickedTab);

                // select the clicked tab
                SelectTab(clickedTab);

                // Play a default sound
                AudioManager.PlayDefaultButtonSound();
            }
        }

        // Locate all VisualElements that have the tab class name
        UQueryBuilder<VisualElement> GetAllTabs()
        {
            return m_TopElement.Query<VisualElement>(className: k_TabClass);
        }

        // De-select a specific tab
        void UnselectTab(VisualElement tab)
        {
            tab.RemoveFromClassList(k_SelectedTabClass);
        }

        void SelectTab(VisualElement tab)
        {
            tab.AddToClassList(k_SelectedTabClass);
        }

        // Select a specific tab by name, e.g. "gold" or "gem" 
        public void SelectTab(string tabName)
        {
            switch (tabName)
            {
                case "gold":
                    ClickTabButton(m_GoldTabButton);
                    ShopEvents.GoldSelected?.Invoke();
                    break;
                case "gem":
                    ClickTabButton(m_GemTabButton);
                    ShopEvents.GemSelected?.Invoke();
                    break;
                case "potion":
                    ClickTabButton(m_PotionTabButton);
                    ShopEvents.PotionSelected?.Invoke();
                    break;
                default:
                    ShopEvents.GoldSelected?.Invoke();
                    break;
            }
        }

        public bool IsTabSelected(VisualElement tab)
        {
            return tab.ClassListContains(k_SelectedTabClass);
        }

        void UnselectOtherTabs(VisualElement tab)
        {

            GetAllTabs().Where(
                (t) => t != tab && IsTabSelected(t)).
                ForEach(UnselectTab);
        }

        // Fill a tab with content
        public void OnShopUpdated(List<ShopItemSO> shopItems)
        {
            if (shopItems == null || shopItems.Count == 0)
                return;

            // generate items for each tab (gold, gems, potions)
            VisualElement parentTab = m_ShopScrollView;

            ScrollView scrollView = parentTab.Q<ScrollView>(className: "unity-scroll-view");
            scrollView.scrollOffset = Vector2.zero;

            parentTab.Clear();

            foreach (ShopItemSO shopItem in shopItems)
            {
                CreateShopItemElement(shopItem, parentTab);
            }
        }

        void CreateShopItemElement(ShopItemSO shopItemData, VisualElement parentElement)
        {
            if (parentElement == null || shopItemData == null || m_ShopItemAsset == null)
                return;

            // instantiate a new Visual Element from a template UXML
            TemplateContainer shopItemElem = m_ShopItemAsset.Instantiate();
            shopItemElem.AddToClassList(k_ShopTemplateContainerClass);

            // sets up the VisualElements and game data per ShopItem
            ShopItemComponent shopItemController = new ShopItemComponent(m_GameIconsData, shopItemData);

            shopItemController.SetVisualElements(shopItemElem);
            shopItemController.SetGameData(shopItemElem);

            shopItemController.RegisterCallbacks();

            parentElement.Add(shopItemElem);

        }
    }
}


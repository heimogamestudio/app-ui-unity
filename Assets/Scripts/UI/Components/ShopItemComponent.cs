using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;


namespace UIToolkitDemo
{
    /// <summary>
    /// Manages the visual representation and interaction logic for a shop item in the UI.
    /// Binds shop item data (such as cost, discount, and content) to UI elements.
    /// </summary>
    public class ShopItemComponent
    {
        // Class selectors for item size (normal or wide)
        const string k_SizeNormalClass = "shop-item__size--normal";
        const string k_SizeWideClass = "shop-item__size--wide";

        // ScriptableObject pairing icons with currency/shop item types
        GameIconsSO m_GameIconsData;
        ShopItemSO m_ShopItemData;

        // visual elements
        Label m_Description;
        VisualElement m_ProductImage;
        VisualElement m_Banner;
        Label m_BannerLabel;
        VisualElement m_ContentCurrency;
        Label m_ContentValue;
        VisualElement m_CostIcon;
        Label m_Cost;
        VisualElement m_DiscountBadge;
        Label m_DiscountLabel;
        VisualElement m_DiscountSlash;
        VisualElement m_DiscountGroup;
        VisualElement m_SizeContainer;
        Label m_DiscountCost;
        Button m_BuyButton;
        VisualElement m_CostIconGroup;
        VisualElement m_DiscountIconGroup;

        Label m_CostFree; 
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameIconsData">ScriptableObject with general icon data.</param>
        /// <param name="shopItemData">ScriptableObject holding the product info.</param>
        public ShopItemComponent(GameIconsSO gameIconsData, ShopItemSO shopItemData)
        {
            m_GameIconsData = gameIconsData;
            m_ShopItemData = shopItemData;
        }

        /// <summary>
        /// Queries and assigns references to the UI elements from the provided TemplateContainer.
        /// </summary>
        public void SetVisualElements(TemplateContainer shopItemElement)
        {
            // query the parts of the ShopItemElement
            m_SizeContainer = shopItemElement.Q("shop-item__container");
            m_Description = shopItemElement.Q<Label>("shop-item__description");
            m_ProductImage = shopItemElement.Q("shop-item__product-image");
            m_Banner = shopItemElement.Q("shop-item__banner");
            m_BannerLabel = shopItemElement.Q<Label>("shop-item__banner-label");
            m_DiscountBadge = shopItemElement.Q("shop-item__discount-badge");
            m_DiscountLabel = shopItemElement.Q<Label>("shop-item__badge-text");
            m_DiscountSlash = shopItemElement.Q("shop-item__discount-slash");
            m_ContentCurrency = shopItemElement.Q("shop-item__content-currency");
            m_ContentValue = shopItemElement.Q<Label>("shop-item__content-value");
            m_CostIcon = shopItemElement.Q("shop-item__cost-icon");
            m_Cost = shopItemElement.Q<Label>("shop-item__cost-price");
            m_CostFree = shopItemElement.Q<Label>("shop-item__cost-free"); 
            // m_DiscountIcon = shopItemElement.Q("shop-item__discount-icon");
            m_DiscountGroup = shopItemElement.Q("shop-item__discount-group");
            m_DiscountCost = shopItemElement.Q<Label>("shop-item__discount-price");
            m_BuyButton = shopItemElement.Q<Button>("shop-item__buy-button");

            m_CostIconGroup = shopItemElement.Q("shop-item__cost-icon-group");
            m_DiscountIconGroup = shopItemElement.Q("shop-item__discount-icon-group");
        }


        /// <summary>
        /// Sets the data source, updates size class, and binds data to UI elements for the shop item.
        /// </summary>
        /// <param name="shopItemElement"></param>
        public void SetGameData(TemplateContainer shopItemElement)
        {
            if (m_GameIconsData == null)
            {
                Debug.LogWarning("[ShopItemComponent] SetGameData: missing GameIcons ScriptableObject data.");
                return;
            }

            if (shopItemElement == null)
            {
                Debug.LogWarning("[ShopItemComponent] SetGameData: missing Template object.");
                return;
            }

            // Set the data source in the top/root element
            m_SizeContainer.dataSource = m_ShopItemData;
            
            UpdateSizeClass();
            
            BindProductInfo();
            
            BindBannerElements();
            
            BindCostElements();
            
            BindDiscountElements();
        }

        /// <summary>
        /// This adds the appropriate USS class. Discounted items use a wider style.
        /// </summary>
        void UpdateSizeClass()
        {
            // Remove both classes first to reset the state
            m_SizeContainer.RemoveFromClassList(k_SizeNormalClass);
            m_SizeContainer.RemoveFromClassList(k_SizeWideClass);

            // Add the appropriate class based on whether the item is discounted
            if (m_ShopItemData.IsDiscounted)
            {
                m_SizeContainer.AddToClassList(k_SizeWideClass);
            }
            else
            {
                m_SizeContainer.AddToClassList(k_SizeNormalClass);
            }
        }
        
        /// <summary>
        /// Binds data from ShopItemSO to description and product image.
        /// </summary>
        void BindProductInfo()
        {
            // 
            m_Description.SetBinding("text", m_ShopItemData.ItemNameLocalized);
            
            m_ProductImage.SetBinding("style.backgroundImage", new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.Icon)),
                bindingMode = BindingMode.ToTarget // One-way binding to the UI
            });
            
            // Bind the background image of m_ContentCurrency (received reward type) to the ContentCurrencyIcon property
            m_ContentCurrency.SetBinding("style.backgroundImage", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.ContentCurrencyIcon)),
                bindingMode = BindingMode.ToTarget // One-way binding from data to UI
            });

            // Bind the text of m_ContentValue (received reward) to the FormattedContentValue property
            m_ContentValue.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.FormattedContentValue)),
                bindingMode = BindingMode.ToTarget // One-way binding from data to UI
            });

        }

        /// <summary>
        /// Shows or hides the banner, which displays additional text about the shop item's
        /// status.
        /// </summary>
        private void BindBannerElements()
        {
            // Bind the banner's visibility to the data source using the named DataBinding
            m_Banner.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.BannerDisplayStyle)),
                bindingMode = BindingMode.ToTarget 
            });

            m_BannerLabel.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.BannerDisplayStyle)),
                bindingMode = BindingMode.ToTarget 
            });
            
            // Bind the promo banner text to the LocalizedString
            m_BannerLabel.SetBinding("text", m_ShopItemData.PromoBannerLocalized);
        }
        
        /// <summary>
        /// Bind elements related to the cost.
        /// </summary>
        void BindCostElements()
        {

            
            // Bind the cost text to FormattedCost
            m_Cost.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.FormattedCost)),
                bindingMode = BindingMode.ToTarget
            });

            // Bind the display style to show/hide regular price
            m_Cost.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.RegularPriceDisplay)),
                bindingMode = BindingMode.ToTarget
            });
            
            // Bind the "Free" text label to use localization
            m_CostFree.SetBinding("text", m_ShopItemData.FreeTextLocalized);
    
            // Bind the display style to show/hide "Free" text
            m_CostFree.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.FreeTextDisplay)),
                bindingMode = BindingMode.ToTarget
            });
            
            // Bind the cost icon to the CostIconSprite property
            m_CostIcon.SetBinding("style.backgroundImage", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.CostIconSprite)),
                bindingMode = BindingMode.ToTarget
            });

            // Show/hide the CostIconGroup based on CostIconGroupDisplayStyle propert
            m_CostIconGroup.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.CostIconGroupDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });
        }
        
        /// <summary>
        /// Show/hide discount related elements based on the shop item's DiscountDisplayStyle.
        /// </summary>
        void BindDiscountElements()
        {
            // Bind the discount label text to DiscountText
            m_DiscountLabel.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountText)),
                bindingMode = BindingMode.ToTarget
            });
            
            // Show/hide the discount label based on DiscountDisplayStyle property
            m_DiscountLabel.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // Bind the discounted cost text to DiscountedCost
            m_DiscountCost.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountedCost)),
                bindingMode = BindingMode.ToTarget
            });

            // Show/hide the DiscountIconGroup based on DiscountIconGroupDisplayStyle property
            m_DiscountIconGroup.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountIconGroupDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });
            
            // Show/hide the discount badge
            m_DiscountBadge.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });
            
            // Show/hide the discount slash
            m_DiscountSlash.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // Show/hide the discount group
            m_DiscountGroup.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });
        }
  
        /// <summary>
        /// Set up interactive buttons.
        /// </summary>
        public void RegisterCallbacks()
        {
            if (m_BuyButton == null)
                return;

            // Store the cost/contents data in each button for later use
            m_BuyButton.userData = m_ShopItemData;
            m_BuyButton.RegisterCallback<ClickEvent>(BuyAction);

            // Prevent the button click from moving the ScrollView
            m_BuyButton.RegisterCallback<PointerMoveEvent>(MovePointerEventHandler);
        }

        /// <summary>
        /// Prevents accidental left-right movement of the mouse from dragging the parent Scrollview
        /// </summary>
        /// <param name="evt">The pointer move event.</param>
        void MovePointerEventHandler(PointerMoveEvent evt)
        {
            evt.StopImmediatePropagation();
        }


        /// <summary>
        /// Clicking the Buy button on the ShopItem triggers a chain of events:
        ///      - ShopItemComponent (click the button) -->
        ///      - ShopController (buy an item) -->
        ///      - GameDataManager (verify funds)-->
        ///      - MagnetFXController (play effect on UI)
        /// </summary>
        /// <param name="evt"></param>
        void BuyAction(ClickEvent evt)
        {
            VisualElement clickedElement = evt.currentTarget as VisualElement;

            // Retrieve the Shop Item Data previously stored in the custom userData
            ShopItemSO shopItemData = clickedElement.userData as ShopItemSO;

            // Get the RootVisualElement 
            VisualElement rootVisualElement = m_SizeContainer.panel.visualTree;

            // Convert to screen position in pixels
            Vector2 clickPos = new Vector2(evt.position.x, evt.position.y);
            Vector2 screenPos = clickPos.GetScreenCoordinate(rootVisualElement);

            // Notify the ShopController (passes ShopItem data + screen position)
            ShopEvents.ShopItemClicked?.Invoke(shopItemData, screenPos);

            AudioManager.PlayDefaultButtonSound();
        }
    }
}


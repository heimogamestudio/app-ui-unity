using UnityEngine.UIElements;
using Unity.Properties;

namespace UIToolkitDemo
{
    /// <summary>
    /// Manages task bar UI for opening SettingsView and ShopView. Updates the gem and gold totals with simple
    /// text animation.
    /// </summary>
    public class OptionsBarView : UIView
    {
        VisualElement m_OptionsButton;
        VisualElement m_ShopGemButton;
        VisualElement m_ShopGoldButton;
        Label m_GoldLabel;
        Label m_GemLabel;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="topElement"></param>
        public OptionsBarView(VisualElement topElement) : base(topElement)
        {
            // Subscribe to the GameDataReceived event, which triggers when new game data is received.
            GameDataManager.GameDataReceived += OnGameDataReceived;

            // Requests the game data from the GameDataManager.
            GameDataManager.GameDataRequested?.Invoke();
        }

        /// <summary>
        /// Handles game data reception and binds the gold and gem values to their respective labels.
        /// </summary>
        /// <param name="gameData">Received game data.</param>
        void OnGameDataReceived(GameData gameData)
        {
            // Data Binding here
            m_GoldLabel.SetBinding("text", new AnimatedTextBinding()
            {
                dataSource = gameData,
                dataSourcePath = new PropertyPath(nameof(GameData.Gold)),
            });
            
            m_GemLabel.SetBinding("text", new AnimatedTextBinding()
            {
                dataSource = gameData,
                dataSourcePath = new PropertyPath(nameof(GameData.Gems)),
            });
        }

        /// <summary>
        /// Unsubscribes from events and unregisters button callbacks.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            GameDataManager.GameDataReceived -= OnGameDataReceived;
            UnregisterButtonCallbacks();
        }

        /// <summary>
        /// Sets the references for visual elements in the options bar UI.
        /// </summary>
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_OptionsButton = m_TopElement.Q("options-bar__button");
            m_ShopGoldButton = m_TopElement.Q("options-bar__gold-button");
            m_ShopGemButton = m_TopElement.Q("options-bar__gem-button");
            m_GoldLabel = m_TopElement.Q<Label>("options-bar__gold-count");
            m_GemLabel = m_TopElement.Q<Label>("options-bar__gem-count");
        }

        /// <summary>
        /// Set up button click events
        /// </summary>
        protected override void RegisterButtonCallbacks()
        {
            m_OptionsButton.RegisterCallback<ClickEvent>(ShowOptionsScreen);
            m_ShopGemButton.RegisterCallback<ClickEvent>(OpenGemShop);
            m_ShopGoldButton.RegisterCallback<ClickEvent>(OpenGoldShop);
        }

        /// <summary>
        /// Unregisters click event handlers for the options and shop buttons.
        /// </summary>
        void UnregisterButtonCallbacks()
        {
            m_OptionsButton.UnregisterCallback<ClickEvent>(ShowOptionsScreen);
            m_ShopGemButton.UnregisterCallback<ClickEvent>(OpenGemShop);
            m_ShopGoldButton.UnregisterCallback<ClickEvent>(OpenGoldShop);
        }

        /// <summary>
        /// Opens the SettingsView when the options button is clicked.
        /// </summary>
        /// <param name="evt">The click event.</param>
        void ShowOptionsScreen(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            MainMenuUIEvents.SettingsScreenShown?.Invoke();
        }

        /// <summary>
        /// Opens the gold shop tab when the gold button is clicked.
        /// </summary>
        /// <param name="evt">The click event.</param>
        void OpenGoldShop(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            // Show the ShopScreen
            MainMenuUIEvents.OptionsBarShopScreenShown?.Invoke();

            // Open the tab to the gold products
            ShopEvents.TabSelected?.Invoke("gold");
        }
        
        /// <summary>
        /// Opens the gem shop tab when the gem button is clicked.
        /// </summary>
        /// <param name="evt">The click event.</param>
        void OpenGemShop(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            // Show the ShopScreen
            MainMenuUIEvents.OptionsBarShopScreenShown?.Invoke();

            // Open the tab to the gem product
            ShopEvents.TabSelected?.Invoke("gem");
        }
    }
}
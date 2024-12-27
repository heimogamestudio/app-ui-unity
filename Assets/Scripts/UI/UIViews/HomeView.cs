using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;


namespace UIToolkitDemo
{
    /// <summary>
    /// Manages the home screen display, showing level information, level selection, and play options. Listens for
    /// changes in locale and updates displayed level information accordingly.
    /// </summary>
    public class HomeView : UIView
    {
        VisualElement m_PlayLevelButton;
        VisualElement m_LevelThumbnail;

        Label m_LevelNumber;
        Label m_LevelLabel;

        LevelSO m_CurrentLevelData;

        ChatView m_ChatView;
        public ChatView ChatView => m_ChatView;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="topElement">The topmost/root element of the VisualTree.</param>
        public HomeView(VisualElement topElement) : base(topElement)
        {
            m_ChatView = new ChatView(topElement);

            HomeEvents.LevelInfoShown += OnShowLevelInfo;

            // Listen to locale changes
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }

        /// <summary>
        /// Sets references to UI elements.
        /// </summary>
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_PlayLevelButton = m_TopElement.Q("home-play__level-button");
            m_LevelLabel = m_TopElement.Q<Label>("home-play__level-name");
            m_LevelNumber = m_TopElement.Q<Label>("home-play__level-number");

            m_LevelThumbnail = m_TopElement.Q("home-play__background");
        }

        /// <summary>
        /// Registers the play button click event to load the game scene.
        /// </summary>
        protected override void RegisterButtonCallbacks()
        {
            m_PlayLevelButton.RegisterCallback<ClickEvent>(ClickPlayButton);
        }

        /// <summary>
        /// Unsubscribe and unregister to prevent memory leaks.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            HomeEvents.LevelInfoShown -= OnShowLevelInfo;
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
            
            m_PlayLevelButton.UnregisterCallback<ClickEvent>(ClickPlayButton);
        }

        /// <summary>
        /// Play a sound and notify any connected play logic.
        /// </summary>
        /// <param name="evt"></param>
        void ClickPlayButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            HomeEvents.PlayButtonClicked?.Invoke();
        }

        // Event-handling Methods

        /// <summary>
        /// Displays the specified level information on the home view.
        /// </summary>
        /// <param name="levelData">The ScriptableObject level data.</param>
        void OnShowLevelInfo(LevelSO levelData)
        {
            if (levelData == null)
                return;

            // Cache a copy for localization updates
            m_CurrentLevelData = levelData;

            ShowLevelInfo(levelData.LevelNumberFormatted, levelData.LevelSubtitle, levelData.Thumbnail);
        }

        /// <summary>
        /// Re-fetch and update localized strings as the locale changes.
        /// </summary>
        /// <param name="locale">The new Locale.</param>
        void OnLocaleChanged(Locale locale)
        {
            ShowLevelInfo(m_CurrentLevelData.LevelNumberFormatted, m_CurrentLevelData.LevelSubtitle,
                m_CurrentLevelData.Thumbnail);
        }

        /// <summary>
        /// Shows the level information
        /// </summary>
        /// <param name="levelNumberFormatted"></param>
        /// <param name="levelName"></param>
        /// <param name="thumbnail"></param>
        public void ShowLevelInfo(string levelNumberFormatted, string levelName, Sprite thumbnail)
        {
            if (m_LevelNumber == null || m_LevelLabel == null || m_LevelThumbnail == null)
                return;

            m_LevelNumber.text = levelNumberFormatted;
            m_LevelLabel.text = levelName;
            m_LevelThumbnail.style.backgroundImage = new StyleBackground(thumbnail);
        }
    }
}
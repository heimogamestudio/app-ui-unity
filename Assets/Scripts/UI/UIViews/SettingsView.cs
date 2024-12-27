using System;
using UnityEngine;
using UnityEngine.UIElements;
using MyUILibrary;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UIToolkitDemo
{
    /// <summary>
    /// This controls general settings for the game. Some of these options are non-functional in this demo but
    /// show how to send data to the GameDataManager.
    /// </summary>
    public class SettingsView : UIView
    {
        // These class selectors hide/show the settings screen overlay; this allows USS transitions to
        // fade the UI on/off.
        const string k_ScreenActiveClass = "settings__screen";
        const string k_ScreenInactiveClass = "settings__screen--inactive";

        // Visual elements
        Button m_BackButton;
        Button m_ResetLevelButton;
        Button m_ResetFundsButton;
        TextField m_PlayerTextfield;
        Toggle m_ExampleToggle;
        DropdownField m_ThemeDropdown;
        DropdownField m_LanguageDropdown;
        Slider m_MusicSlider;
        Slider m_SfxSlider;
        SlideToggle m_SlideToggle;
        RadioButtonGroup m_FrameRateRadioButtonsGroup;
        VisualElement m_ScreenContainer; // Top UI element for transitions

        // Temporary storage to send settings data back to SettingsController
        GameData m_LocalUISettings = new GameData();

        LocalizationManager m_LocalizationManager;

        // Because they are localized and not fixed strings, we use these arrays to map the dropdown options to 
        // their internal values. We use arrays (not dictionaries) to preserve the intended order.

        // Dropdown options for language selection (names match first name of Locale)
        public static readonly string[] LanguageKeys = { "English", "Spanish", "French", "Danish" };

        // Dropdown options for Theme selection (names match Theme Style Sheets)
        public static readonly string[] ThemeOptionKeys = { "Default", "Halloween", "Christmas" };

        // Constructor and life cycle methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="topElement"></param>
        public SettingsView(VisualElement topElement) : base(topElement)
        {
            // Sets m_SettingsData using previously saved data
            SettingsEvents.GameDataLoaded += OnGameDataLoaded;

            base.SetVisualElements();

            // Hide/disable by default
            m_ScreenContainer.AddToClassList(k_ScreenInactiveClass);
            m_ScreenContainer.RemoveFromClassList(k_ScreenActiveClass);

            m_LocalizationManager = new LocalizationManager();

            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
        }

        /// <summary>
        /// Disposes of the <see cref="SettingsView"/> instance and unregisters event handlers.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            SettingsEvents.GameDataLoaded -= OnGameDataLoaded;
            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;

            UnregisterButtonCallbacks();
        }

        /// <summary>
        /// Shows the settings view and triggers UI transitions.
        /// </summary>
        public override void Show()
        {
            base.Show();

            // Use styles to fade in with transition
            m_ScreenContainer.RemoveFromClassList(k_ScreenInactiveClass);
            m_ScreenContainer.AddToClassList(k_ScreenActiveClass);

            // Notify GameDataManager
            SettingsEvents.SettingsShown?.Invoke();
        }

        // UI Initialization methods

        /// <summary>
        /// Initializes and caches the visual UI elements from the UI document.
        /// </summary>
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_BackButton = m_TopElement.Q<Button>("settings__panel-back-button");
            m_ResetLevelButton = m_TopElement.Q<Button>("settings__social-button1");
            m_ResetFundsButton = m_TopElement.Q<Button>("settings__social-button2");
            m_PlayerTextfield = m_TopElement.Q<TextField>("settings__player-textfield");
            m_ExampleToggle = m_TopElement.Q<Toggle>("settings__toggle");
            m_ThemeDropdown = m_TopElement.Q<DropdownField>("settings__theme-dropdown");
            m_LanguageDropdown = m_TopElement.Q<DropdownField>("settings__dropdown");
            m_MusicSlider = m_TopElement.Q<Slider>("settings__slider1");
            m_SfxSlider = m_TopElement.Q<Slider>("settings__slider2");
            m_SlideToggle = m_TopElement.Q<SlideToggle>("settings__slide-toggle");
            m_FrameRateRadioButtonsGroup = m_TopElement.Q<RadioButtonGroup>("settings__radio-button-group");

            m_ScreenContainer = m_TopElement.Q<VisualElement>("settings__screen");

            UpdateLocalization();
        }

        /// <summary>
        /// Registers callback methods for UI element events.
        /// </summary>
        protected override void RegisterButtonCallbacks()
        {
            m_BackButton.RegisterCallback<ClickEvent>(CloseWindow);

            m_ResetLevelButton.RegisterCallback<ClickEvent>(ResetLevel);
            m_ResetFundsButton.RegisterCallback<ClickEvent>(ResetFunds);

            m_PlayerTextfield.RegisterCallback<KeyDownEvent>(SetPlayerTextfield);
            m_ThemeDropdown.RegisterValueChangedCallback(ChangeThemeDropdown);
            m_ThemeDropdown.RegisterCallback<PointerDownEvent>(evt => AudioManager.PlayDefaultButtonSound());

            m_LanguageDropdown.RegisterValueChangedCallback(ChangeLanguageDropdown);

            m_LanguageDropdown.RegisterCallback<PointerDownEvent>(evt => AudioManager.PlayDefaultButtonSound());

            m_MusicSlider.RegisterValueChangedCallback(ChangeMusicVolume);
            m_MusicSlider.RegisterCallback<PointerCaptureOutEvent>(evt =>
                SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings));
            m_MusicSlider.RegisterCallback<PointerDownEvent>(evt => AudioManager.PlayDefaultButtonSound());

            m_SfxSlider.RegisterValueChangedCallback(ChangeSfxVolume);
            m_SfxSlider.RegisterCallback<PointerCaptureOutEvent>(evt =>
                SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings));
            m_SfxSlider.RegisterCallback<PointerDownEvent>(evt => AudioManager.PlayDefaultButtonSound());

            m_ExampleToggle.RegisterValueChangedCallback(ChangeToggle);
            m_ExampleToggle.RegisterCallback<ClickEvent>(evt => AudioManager.PlayDefaultButtonSound());

            m_SlideToggle.RegisterValueChangedCallback(ChangeSlideToggle);
            m_SlideToggle.RegisterCallback<ClickEvent>(evt => AudioManager.PlayDefaultButtonSound());

            m_FrameRateRadioButtonsGroup.RegisterCallback<ChangeEvent<int>>(ChangeRadioButton);
        }

        /// <summary>
        /// Unregisters callback methods from UI elements to prevent memory leaks.
        /// </summary>
        void UnregisterButtonCallbacks()
        {
            // Unregister all callbacks from UI elements
            m_BackButton?.UnregisterCallback<ClickEvent>(CloseWindow);
            m_ResetLevelButton?.UnregisterCallback<ClickEvent>(ResetLevel);
            m_ResetFundsButton?.UnregisterCallback<ClickEvent>(ResetFunds);
            m_PlayerTextfield?.UnregisterCallback<KeyDownEvent>(SetPlayerTextfield);
            m_ThemeDropdown?.UnregisterValueChangedCallback(ChangeThemeDropdown);
            m_LanguageDropdown?.UnregisterValueChangedCallback(ChangeLanguageDropdown);
            m_MusicSlider?.UnregisterValueChangedCallback(ChangeMusicVolume);
            m_SfxSlider?.UnregisterValueChangedCallback(ChangeSfxVolume);
            m_ExampleToggle?.UnregisterValueChangedCallback(ChangeToggle);
            m_SlideToggle?.UnregisterValueChangedCallback(ChangeSlideToggle);
            m_FrameRateRadioButtonsGroup?.UnregisterCallback<ChangeEvent<int>>(ChangeRadioButton);
        }

        // Localization methods

        /// <summary>
        /// Updates the localization settings based on the selected language.
        /// </summary>
        void UpdateLocalization()
        {
            // Don't proceed if dropdown is not initialized
            if (m_LanguageDropdown == null)
                return;

            // First set the locale based on saved selection
            if (!string.IsNullOrEmpty(m_LocalUISettings.LanguageSelection))
            {
                string localeCode = LocalizationManager.GetLocaleCode(m_LocalUISettings.LanguageSelection);
                m_LocalizationManager?.SetLocale(localeCode);

                // Wait one frame to ensure locale is set before updating text
                m_TopElement.schedule.Execute(() => { UpdateLocalizedText(); });
            }
            else
            {
                UpdateLocalizedText();
            }
        }

        /// <summary>
        /// Updates the localized text for dynamic UI elements not set up in UI Builder (theme dropdown, language dropdown, slide toggle, and frame rate
        /// radio button)
        /// </summary>
        void UpdateLocalizedText()
        {
            // Update the theme dropdown options
            string[] themeChoices = new string[]
            {
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_ThemeDropdown_Option1"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_ThemeDropdown_Option2"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_ThemeDropdown_Option3")
            };
            
            m_ThemeDropdown.UpdateLocalizedChoices(themeChoices, m_LocalUISettings.Theme, ThemeOptionKeys);

            // Get localized names for languages using the correct keys from your localization table
            string[] languageChoices = new string[]
            {
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_LanguageDropdown_English"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_LanguageDropdown_Spanish"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_LanguageDropdown_French"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_LanguageDropdown_Danish")
            };

            m_LanguageDropdown.UpdateLocalizedChoices(languageChoices, m_LocalUISettings.LanguageSelection,
                LanguageKeys);

            // Update the on/off Slide Toggle labels
            string onLabel =
                m_SlideToggle.OffLabel =
                    LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                        "Settings_FpsSlideToggle_Off");

            m_SlideToggle.OnLabel =
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Settings_FpsSlideToggle_On");
            m_SlideToggle.SetValueWithoutNotify(m_SlideToggle.value);

            // Update the Max frame rate radio button label
            var radioButtons = m_FrameRateRadioButtonsGroup.Query<RadioButton>().ToList();
            radioButtons[0].text =
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_FrameRateRadioButtons_Max");
        }

        /// <summary>
        /// Event handler triggered when changing Locales.
        /// </summary>
        /// <param name="newLocale"></param>
        void OnSelectedLocaleChanged(Locale newLocale)
        {
            UpdateLocalizedText();
        }

        // General event-handling methods

        /// <summary>
        /// Handles the event when game data is loaded, updating UI elements with saved values.
        /// </summary>
        /// <param name="loadedGameData">The loaded game data.</param>
        void OnGameDataLoaded(GameData loadedGameData)
        {
            if (loadedGameData == null)
                return;

            m_LocalUISettings = loadedGameData;

            // Update non-localized UI elements first
            m_PlayerTextfield.value = loadedGameData.UserName;
            m_ThemeDropdown.value = loadedGameData.Theme;
            m_FrameRateRadioButtonsGroup.value = loadedGameData.TargetFrameRateSelection;
            m_MusicSlider.value = loadedGameData.MusicVolume;
            m_SfxSlider.value = loadedGameData.SfxVolume;
            m_SlideToggle.value = loadedGameData.IsFpsCounterEnabled;
            m_ExampleToggle.value = loadedGameData.IsToggled;

            // Then handle localization, which will update the language dropdown
            UpdateLocalization();

            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }

        /// <summary>
        /// Closes the settings window and triggers UI transitions.
        /// </summary>
        /// <param name="evt">The click event triggering the close action.</param>
        void CloseWindow(ClickEvent evt)
        {
            m_ScreenContainer.RemoveFromClassList(k_ScreenActiveClass);
            m_ScreenContainer.AddToClassList(k_ScreenInactiveClass);

            AudioManager.PlayDefaultButtonSound();

            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);

            Hide();
        }

        /// <summary>
        /// Handles changes to the player name TextField when hitting Return/Enter.
        /// </summary>
        /// <param name="evt"></param>
        void SetPlayerTextfield(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return && m_LocalUISettings != null)
            {
                m_LocalUISettings.UserName = m_PlayerTextfield.text;
                SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
            }
        }

        /// <summary>
        /// Handles changes to the FPS counter Slide Toggle.
        /// </summary>
        /// <param name="evt"></param>
        void ChangeSlideToggle(ChangeEvent<bool> evt)
        {
            // Toggles the SlideToggle value (enables/disables the FPS counter)
            m_LocalUISettings.IsFpsCounterEnabled = evt.newValue;

            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        // Callback function for Toggle change event
        void ChangeToggle(ChangeEvent<bool> evt)
        {
            // non-functional setting for demo purposes
            m_LocalUISettings.IsToggled = evt.newValue;

            // notify the GameDataManager
            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }

        /// <summary>
        /// Handles changes to the sound effects volume slider.
        /// </summary>
        /// <param name="evt"></param>
        void ChangeSfxVolume(ChangeEvent<float> evt)
        {
            evt.StopPropagation();
            m_LocalUISettings.SfxVolume = evt.newValue;
        }

        /// <summary>
        /// Handles changes to the music volume slider.
        /// </summary>
        /// <param name="evt">The change event containing the new float value.</param>
        void ChangeMusicVolume(ChangeEvent<float> evt)
        {
            evt.StopPropagation();
            m_LocalUISettings.MusicVolume = evt.newValue;
        }

        /// <summary>
        /// Handles changes to the theme selection dropdown.
        /// </summary>
        /// <param name="evt">The change event containing the new string value (not used here).</param>
        void ChangeThemeDropdown(ChangeEvent<string> evt)
        {
            // Get the selected index from the dropdown
            int selectedIndex = m_ThemeDropdown.index;

            // Validate the selected index
            if (selectedIndex >= 0 && selectedIndex < ThemeOptionKeys.Length)
            {
                // Map the index to the logical key
                m_LocalUISettings.Theme = ThemeOptionKeys[selectedIndex];
            }
            else
            {
                // Handle error or default case
                m_LocalUISettings.Theme = ThemeOptionKeys[0]; // Default theme (original name)
            }

            // Notify other components of the change
            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }

        /// <summary>
        /// Handles changes to the language selection dropdown.
        /// </summary>
        /// <param name="evt">The change event containing the new string value (not used here).</param>
        void ChangeLanguageDropdown(ChangeEvent<string> evt)
        {
            int selectedIndex = m_LanguageDropdown.index;

            if (selectedIndex >= 0 && selectedIndex < LanguageKeys.Length)
            {
                m_LocalUISettings.LanguageSelection = LanguageKeys[selectedIndex];

                // Set the locale first
                string localeCode = LocalizationManager.GetLocaleCode(m_LocalUISettings.LanguageSelection);
                m_LocalizationManager?.SetLocale(localeCode);

                // Then update the UI
                UpdateLocalizedText();
            }

            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }


        /// <summary>
        /// Handles changes to the frame rate radio button selection.
        /// </summary>
        /// <param name="evt">The change event containing the new selected index.</param>
        void ChangeRadioButton(ChangeEvent<int> evt)
        {
            AudioManager.PlayDefaultButtonSound();

            // non-functional setting for demo purposes
            m_LocalUISettings.TargetFrameRateSelection = evt.newValue;

            // notify the GameDataManager
            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }

        /// <summary>
        /// Handles the click event for the Reset Level button.
        /// </summary>
        /// <param name="evt">The click event triggering the reset action.</param>
        void ResetLevel(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            SettingsEvents.PlayerLevelReset?.Invoke();
        }

        /// <summary>
        /// Handles the click event for the Reset Funds button.
        /// </summary>
        /// <param name="evt">The click event triggering the reset action.</param>
        void ResetFunds(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            SettingsEvents.PlayerFundsReset?.Invoke();
        }
    }
}
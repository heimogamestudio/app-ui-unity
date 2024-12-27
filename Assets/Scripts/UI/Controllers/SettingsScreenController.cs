using UnityEngine;

namespace UIToolkitDemo
{
    // <summary>
    /// Manages the settings data and controls the flow of this data 
    /// between the SaveManager and the UI.
    /// </summary>
    public class SettingsScreenController : MonoBehaviour
    {
        GameData m_SettingsData;
        
        // Manages localization settings
        LocalizationManager m_LocalizationManager;

        // Aspect ratio for Theme
        MediaAspectRatio m_MediaAspectRatio = MediaAspectRatio.Undefined;

        [SerializeField] bool m_Debug;

        void OnEnable()
        {
            MediaQueryEvents.ResolutionUpdated += OnResolutionUpdated;
            SettingsEvents.UIGameDataUpdated += OnUISettingsUpdated;
            SaveManager.GameDataLoaded += OnGameDataLoaded;

            m_LocalizationManager = new LocalizationManager(m_Debug);
        }

        void OnDisable()
        {
            MediaQueryEvents.ResolutionUpdated -= OnResolutionUpdated;
            SettingsEvents.UIGameDataUpdated -= OnUISettingsUpdated;
            SaveManager.GameDataLoaded -= OnGameDataLoaded;
        }

        void Awake()
        {
            if (m_Debug)
                m_LocalizationManager.LogAvailableLocales();
        }

        /// <summary>
        /// Target frame rate based on radio button selection ( -1 = as fast as possible, 60fps, 30fps)
        /// </summary>
        /// <param name="selectedIndex"></param>
        void SelectTargetFrameRate(int selectedIndex)
        {
            // Convert button index to target frame rate
            switch (selectedIndex)
            {
                case 0:
                    SettingsEvents.TargetFrameRateSet?.Invoke(-1);
                    break;
                case 1:
                    SettingsEvents.TargetFrameRateSet?.Invoke(60);
                    break;
                case 2:
                    SettingsEvents.TargetFrameRateSet?.Invoke(30);
                    break;
                default:
                    SettingsEvents.TargetFrameRateSet?.Invoke(60);
                    break;
            }
        }

        /// <summary>
        /// Handle updated Settings Data
        /// </summary>
        /// <param name="newSettingsData"></param>
        
        void OnUISettingsUpdated(GameData newSettingsData)
        {
            if (newSettingsData == null)
                return;

            m_SettingsData = newSettingsData;

            // Toggle the Fps Counter based on slide toggle position
            SettingsEvents.FpsCounterToggled?.Invoke(m_SettingsData.IsFpsCounterEnabled);
            SelectTargetFrameRate(m_SettingsData.TargetFrameRateSelection);

            // Notify the GameDataManager and other listeners
            SettingsEvents.SettingsUpdated?.Invoke(m_SettingsData);

            ApplyThemeSettings();

            ApplyLocalizationSettings();
        }

        /// <summary>
        /// Sync loaded data from SaveManager to UI
        /// </summary>
        /// <param name="gameData"></param>
        void OnGameDataLoaded(GameData gameData)
        {
            if (gameData == null)
                return;

            m_SettingsData = gameData;
            SettingsEvents.GameDataLoaded?.Invoke(m_SettingsData);
        }

        /// <summary>
        /// Store portrait/landscape for Theme
        /// </summary>
        /// <param name="resolution"></param>
        void OnResolutionUpdated(Vector2 resolution)
        {
            m_MediaAspectRatio = MediaQuery.CalculateAspectRatio(resolution);
        }
        
        
        /// <summary>
        /// Sets the Locale based on language selection in m_SettingsData.
        /// </summary>
        void ApplyLocalizationSettings()
        {
            m_LocalizationManager.SetLocale(m_SettingsData.LanguageSelection);
        }

        /// <summary>
        /// Sets the Theme based n the m_SettingsData
        /// </summary>
        void ApplyThemeSettings()
        {
            // SettingsData stores the Theme as a string key. The basename is the aspect ratio (Landscape or Portrait) plus
            // a modifier for the seasonal decorations. This sample includes 6 themes (Landscape--Default, Landscape--Halloween,
            // Landscape--Christmas, Portrait--Default, Portrait--Halloween, Portrait--Christmas).

            string newTheme = m_MediaAspectRatio.ToString() + "--" + m_SettingsData.Theme;
            ThemeEvents.ThemeChanged(newTheme);
        }
    }
}
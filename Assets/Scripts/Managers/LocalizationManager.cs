using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEditor;

namespace UIToolkitDemo
{
    /// <summary>
    /// Class to help manage Localization settings.
    /// </summary>
    public class LocalizationManager
    {
        [SerializeField] bool m_Debug;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="debug">Flag to log debug messages at the console.</param>
        public LocalizationManager(bool debug = false)
        {
            m_Debug = debug;
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // Reset the localization system to prevent errors when exiting Play mode
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                LocalizationSettings.Instance.ResetState();
            }
        }

        /// <summary>
        /// Deconstructor.
        /// </summary>
        ~LocalizationManager()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
#endif
        public void SetLocale(string localeName)
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;

            // Trim the input localeName to remove any leading/trailing whitespace
            string inputLocaleName = localeName.Trim();

            foreach (var locale in locales)
            {
                // Get the display name up to the first '(', trimming any whitespace
                string localeDisplayName = locale.name.Split('(')[0].Trim();

                // Check if the locale display name starts with the input locale name
                if (localeDisplayName.StartsWith(inputLocaleName, StringComparison.OrdinalIgnoreCase))
                {
                    LocalizationSettings.SelectedLocale = locale;

                    if (m_Debug)
                        Debug.Log("Locale changed to: " + locale.name);
                    return;
                }
            }

            // If no matching locale is found, log a warning if debugging is enabled
            if (m_Debug)
                Debug.LogWarning("Locale not found for name: " + localeName);
        }

        /// <summary>
        /// Locales use a formal name plus a two-letter code in parentheses, e.g. "English (en)". This
        /// returns the two-letter code when only given the first part of the Locale name ("English" -> "en")
        /// </summary>
        /// <param name="localeName">The Locale to convert.</param>
        /// <returns></returns>
        // Method to retrieve the locale code from the locale name
        public static string GetLocaleCode(string localeName)
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;

            // Strip off the part in parentheses (e.g., "English (en)" -> "English")
            string strippedLocaleName = localeName.Split('(')[0].Trim();

            foreach (var locale in locales)
            {
                string localeDisplayName = locale.name.Split('(')[0].Trim();

                if (localeDisplayName.Equals(strippedLocaleName, StringComparison.OrdinalIgnoreCase))
                {
                    return locale.Identifier.Code; // Return the locale code (e.g., "en", "fr")
                }
            }

            return string.Empty; // Fallback empty string
        }

        /// <summary>
        /// Logs the available locales for debugging
        /// </summary>
        public void LogAvailableLocales()
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;

            Debug.Log("Available Locales:");
            foreach (var locale in locales)
            {
                Debug.Log($"Locale Name: {locale.name}, Locale Code: {locale.Identifier.Code}");
            }
        }
    }
}
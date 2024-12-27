using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;


namespace UIToolkitDemo
{
    // Pairs a Theme StyleSheet with a string 
    [Serializable]
    public struct ThemeSettings
    {
        public string theme;
        public ThemeStyleSheet tss;
        public PanelSettings panelSettings;
    }

    // This component changes the Theme Style Sheet (from the Settings Screen or MediaQuery).
    // Use this for changing multiple USS stylesheets at once. Possible applications include seasonal
    // variations (e.g. Christmas, Halloween) or screen size (portrait). 

    [ExecuteInEditMode]
    public class ThemeManager : MonoBehaviour
    {
        [Tooltip("Reference to the UI Document to update for themes")]
        [SerializeField] UIDocument m_Document;
        [Tooltip("Theme is a string key, ThemeSettings, and Panel Settings")]
        [SerializeField] List<ThemeSettings> m_ThemeSettings;
        [SerializeField] bool m_Debug;

        string m_CurrentTheme;

        void OnEnable()
        {
            if (m_ThemeSettings.Count == 0)
            {
                Debug.LogWarning("[ThemeManager]: Add ThemeSettings to set themes");
                return;
            }
            // Theme changed directly from SettingsScreen
            ThemeEvents.ThemeChanged += OnThemeChanged;

            // Theme changed via viewport sizes
            MediaQueryEvents.AspectRatioUpdated += OnAspectRatioUpdated;

            // Default to the first theme
            m_CurrentTheme = m_ThemeSettings[0].theme;
        }

        void OnDisable()
        {
            ThemeEvents.ThemeChanged -= OnThemeChanged;
            MediaQueryEvents.AspectRatioUpdated -= OnAspectRatioUpdated;
        }

        // Change the Theme Stylesheet in the PanelSettings asset
        public void ApplyTheme(string theme)
        {
            if (m_Document == null)
            {
                m_Document = FindFirstObjectByType<UIDocument>();
            }

            if (m_Document == null)
            {
                if (m_Debug)
                {
                    Debug.LogWarning("[ThemeManager] ApplyTheme: Unassigned UI Document.");
                }
                return;
            }

            SetPanelSettings(theme);

            SetThemeStyleSheet(theme);

            m_CurrentTheme = theme;
        }

        void SetThemeStyleSheet(string theme)
        {
            ThemeStyleSheet tss = GetThemeStyleSheet(theme);

            if (tss != null)
            {
                m_Document.panelSettings.themeStyleSheet = tss;

                if (m_Debug)
                {
                    Debug.Log("[ThemeManager] Applying theme style sheet: " + tss.name);
                }
            }
            else if (m_Debug)
            {
                Debug.LogWarning("[ThemeManager] ApplyTheme: Found no matching theme style sheet for " + theme);
            }
        }

        // Apply the theme's corresponding PanelSettings to the UI Document
        void SetPanelSettings(string theme)
        {
            PanelSettings panelSettings = GetPanelSettings(theme);

            if (panelSettings != null)
            {
                m_Document.panelSettings = panelSettings;
            }
            else if (m_Debug)
            {
                Debug.LogWarning("[ThemeManager] ApplyTheme: Found no matching PanelSettings for " + theme);
            }
        }

        // Find the corresponding Theme Style Sheet with a given string
        ThemeStyleSheet GetThemeStyleSheet(string themeName)
        {
            int index = GetThemeIndex(themeName);
            if (index < 0)
            {
                Debug.LogWarning("[ThemeManager] GetThemeStyleSheet: Invalid theme name" + themeName);
                return null;
            }
            return m_ThemeSettings[index].tss;
        }

        // Returns the corresponding PanelSettings for a given theme
        PanelSettings GetPanelSettings(string themeName)
        {
            int index = GetThemeIndex(themeName);

            if (index < 0)
            {
                Debug.LogWarning("[ThemeManager] GetPanelSettings: Invalid theme name" + themeName);
                return null;
            }
            return m_ThemeSettings[index].panelSettings;
        }

        // Returns the corresponding index of a given theme
        int GetThemeIndex(string themeName)
        {
            if (string.IsNullOrEmpty(themeName))
                return -1;

            // Returns index from ThemeSettings (or -1 if not found)
            int index = m_ThemeSettings.FindIndex(x => x.theme == themeName);

            return index;
        }

        public static string GetPrefix(string input, string delimiter)
        {
            int lastIndex = input.LastIndexOf(delimiter);
            if (lastIndex == -1)
            {
                return input; // Delimiter not found, return the original string
            }
            return input.Substring(0, lastIndex);
        }

        public static string GetSuffix(string input, string delimiter)
        {
            int lastIndex = input.LastIndexOf(delimiter);
            if (lastIndex == -1)
            {
                return string.Empty; // Delimiter not found, return an empty string
            }
            return input.Substring(lastIndex);
        }

        // Event-raising methods

        void OnThemeChanged(string newTheme)
        {
            ApplyTheme(newTheme);
            AudioManager.PlayAltButtonSound();

            if (m_Debug)
            {
                Debug.Log("[ThemeManager] OnThemeChanged: " + newTheme);
            }
        }

        // Re-apply Theme when switching between Portrait and Landscape
        void OnAspectRatioUpdated(MediaAspectRatio mediaAspectRatio)
        {
            // Save the suffix to Default, Christmas, or Halloween
            string suffix = GetSuffix(m_CurrentTheme, "--");

            // Add Portrait or Landscape as the basename
            string newThemeName = mediaAspectRatio.ToString() + suffix; 

            ApplyTheme(newThemeName);

            if (m_Debug)
            {
                Debug.Log("[ThemeManager] OnAspectRatioUpdated: " + newThemeName);
            }
        }
    }
}

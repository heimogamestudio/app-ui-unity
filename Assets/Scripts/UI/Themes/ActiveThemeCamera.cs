using System.Collections.Generic;
using UnityEngine;
using System;


namespace UIToolkitDemo
{
    // Pairs a Theme StyleSheet with a string 
    [Serializable]
    public struct CameraTheme
    {
        public Camera camera;
        public string theme;
    }

    /// <summary>
    /// This component pairs a specific camera with a specific theme, enabling the
    /// corresponding camera when switching.
    /// </summary>
    [ExecuteInEditMode]
    public class ActiveThemeCamera : MonoBehaviour
    {
        [Tooltip("Pairs a camera with a theme.")]
        [SerializeField] List<CameraTheme> m_CameraThemes;
        [Tooltip("Sends a Theme Event to notify other components of the updated camera.")]
        [SerializeField] bool m_SendEvent;
        [Tooltip("Logs debug messages at the console.")]
        [SerializeField] bool m_Debug;

        string m_CurrentTheme;
        Camera m_ActiveCamera;

        public List<CameraTheme> CameraThemes => m_CameraThemes;

        public Camera ActiveCamera => m_ActiveCamera;

        void OnEnable()
        {
            if (m_CameraThemes.Count == 0)
            {
                Debug.LogWarning("[ActiveThemeCamera]: Add CameraThemes to toggle theme cameras");
                return;
            }

            ThemeEvents.ThemeChanged += OnThemeChanged;

            MediaQueryEvents.AspectRatioUpdated += OnAspectRatioUpdated;

            m_ActiveCamera = m_CameraThemes[0].camera;
            m_CurrentTheme = m_CameraThemes[0].theme;
        }


        void OnDisable()
        {
            ThemeEvents.ThemeChanged -= OnThemeChanged;
            MediaQueryEvents.AspectRatioUpdated -= OnAspectRatioUpdated;

        }

        public void ShowCamera(int index)
        {
            for (int i = 0; i < m_CameraThemes.Count; i++)
            {
                m_CameraThemes[i].camera.gameObject.SetActive(false);

                if (index == i)
                    m_ActiveCamera = m_CameraThemes[i].camera;
            }

            m_ActiveCamera.gameObject.SetActive(true);

            if (m_Debug)
                Debug.Log("[Active Theme Camera]: " + m_ActiveCamera.name);

            if (m_SendEvent)
                ThemeEvents.CameraUpdated?.Invoke(m_ActiveCamera);

        }

        // Event-handling methods

        void OnThemeChanged(string themeName)
        {
            int index = m_CameraThemes.FindIndex(x => x.theme == themeName);
            ShowCamera(index);
        }

        // Apply Landscape or Portrait Theme StyleSheets 
        void OnAspectRatioUpdated(MediaAspectRatio mediaAspectRatio)
        {
            // Save the suffix to Default, Christmas, or Halloween
            string suffix = ThemeManager.GetSuffix(m_CurrentTheme, "--");

            // Add Portrait or Landscape as the basename
            string newThemeName = mediaAspectRatio.ToString() + suffix;

            int index = m_CameraThemes.FindIndex(x => x.theme == newThemeName);


            ShowCamera(index);
        }
    }
}

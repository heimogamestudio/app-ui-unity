using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace UIToolkitDemo
{
    // Categorize by aspect ratio
    public enum MediaAspectRatio
    {
        Undefined,
        Landscape,
        Portrait
    }

    [ExecuteInEditMode]
    public class MediaQuery : MonoBehaviour
    {
        [SerializeField] UIDocument m_Document;

        // Minimum aspect to be considered landscape
        public const float k_LandscapeMin = 1.2f;

        // Stores the current screen resolution
        Vector2 m_CurrentResolution;

        // Landscape, Portrait, or Undefined
        MediaAspectRatio m_CurrentAspectRatio;

        public Vector2 CurrentResolution => m_CurrentResolution;

        void OnEnable()
        {
            if (m_Document == null)
            {
                Debug.Log("[MediaQuery]: Assign UI Document.");
                return;
            }

            VisualElement root = m_Document.rootVisualElement;

            if (root != null)
                root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

            QueryResolution();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        void Start()
        {
            QueryResolution();
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            UpdateResolution();
        }

        // Update if resolution changed from previous
        public void QueryResolution()
        {

            Vector2 newResolution = new Vector2(Screen.width, Screen.height);

            if (newResolution != m_CurrentResolution)
            {
                m_CurrentResolution = newResolution;
                MediaQueryEvents.ResolutionUpdated?.Invoke(newResolution);
            }

            MediaAspectRatio newAspectRatio = CalculateAspectRatio(newResolution);

            if (newAspectRatio != m_CurrentAspectRatio)
            {
                m_CurrentAspectRatio = newAspectRatio;
                MediaQueryEvents.AspectRatioUpdated?.Invoke(newAspectRatio);
            }
        }

        // Force update resolution and aspect ratio
        public void UpdateResolution()
        {
            Vector2 newResolution = new Vector2(Screen.width, Screen.height);
            MediaQueryEvents.ResolutionUpdated?.Invoke(newResolution);
            MediaAspectRatio newAspectRatio = CalculateAspectRatio(newResolution);
            MediaQueryEvents.AspectRatioUpdated?.Invoke(newAspectRatio);
        }

        public static MediaAspectRatio CalculateAspectRatio(Vector2 resolution)
        {
            if (Math.Abs(resolution.y) < float.Epsilon)
            {
                Debug.LogWarning("[MediaQuery] CalculateAspectRatio: Height is zero. Cannot calculate aspect ratio.");
                return MediaAspectRatio.Undefined;
            }

            float aspectRatio = resolution.x / resolution.y;

            if (aspectRatio >= k_LandscapeMin)
            {
                return MediaAspectRatio.Landscape;
            }
            else
            {
                return MediaAspectRatio.Portrait;
            }
        }

        public static MediaAspectRatio CalculateAspectRatio(float width, float height)
        {
            return CalculateAspectRatio(new Vector2(width, height));
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateResolution();
        }
    }
}


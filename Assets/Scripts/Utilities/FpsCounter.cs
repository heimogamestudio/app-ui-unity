using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Simple fps counter.
/// </summary>
///
namespace UIToolkitDemo
{

    public class FpsCounter : MonoBehaviour
    {
        public const int k_TargetFrameRate = 60; // 60 for mobile platforms, -1 for fast as possible
        public const int k_BufferSize = 50;  // Number of frames to sample

        [SerializeField] UIDocument m_Document;

        float m_FpsValue;
        int m_CurrentIndex;
        float[] m_DeltaTimeBuffer;

        Label m_FpsLabel;
        bool m_IsEnabled;

        public float FpsValue => m_FpsValue;

        // MonoBehaviour event messages
        void Awake()
        {
            m_DeltaTimeBuffer = new float[k_BufferSize];
            Application.targetFrameRate = k_TargetFrameRate;
        }

        void OnEnable()
        {
            SettingsEvents.FpsCounterToggled += OnFpsCounterToggled;
            SettingsEvents.TargetFrameRateSet += OnTargetFrameRateSet;

            var root = m_Document.rootVisualElement;

            m_FpsLabel = root.Q<Label>("fps-counter");

            if (m_FpsLabel == null)
            {
                Debug.LogWarning("[FPSCounter]: Display label is null.");
                return;
            }
        }

        void OnDisable()
        {
            SettingsEvents.FpsCounterToggled -= OnFpsCounterToggled;
            SettingsEvents.TargetFrameRateSet -= OnTargetFrameRateSet;
        }

        void Update()
        {
            if (m_IsEnabled)
            {
                m_DeltaTimeBuffer[m_CurrentIndex] = Time.deltaTime;
                m_CurrentIndex = (m_CurrentIndex + 1) % m_DeltaTimeBuffer.Length;
                m_FpsValue = Mathf.RoundToInt(CalculateFps());

                m_FpsLabel.text = $"FPS: {m_FpsValue}";
            }

        }

        // Methods
        float CalculateFps()
        {
            float totalTime = 0f;
            foreach (float deltaTime in m_DeltaTimeBuffer)
            {
                totalTime += deltaTime;
            }

            return m_DeltaTimeBuffer.Length / totalTime;
        }

        // Event-handling methods
        void OnFpsCounterToggled(bool state)
        {
            m_IsEnabled = state;
            m_FpsLabel.style.visibility = (state) ? Visibility.Visible : Visibility.Hidden;
        }

        // Set the target frame rate:  -1 = as fast as possible (PC) or 60/30 fps (mobile) 
        void OnTargetFrameRateSet(int newFrameRate)
        {
            Application.targetFrameRate = newFrameRate;
        }
    }
}

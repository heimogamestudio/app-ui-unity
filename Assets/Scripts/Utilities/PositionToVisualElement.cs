using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{

    /// Aligns a GameObject's position to a specified VisualElement

    public class PositionToVisualElement : MonoBehaviour
    {
        [Header("Transform")]
        [SerializeField] GameObject m_ObjectToMove;

        [Header("Camera parameters")]
        [SerializeField] Camera m_Camera;
        [SerializeField] float m_Depth = 10f;

        [Header("UI Target")]
        [SerializeField] UIDocument m_Document;
        [SerializeField] string m_ElementName;

        VisualElement m_TargetElement;

        void OnEnable()
        {
            if (m_Document == null)
            {
                Debug.LogError("[PositionToVisualElement]: UIDocument is not assigned.");
                return;
            }

            VisualElement root = m_Document.rootVisualElement;
            m_TargetElement = root.Q<VisualElement>(name: m_ElementName);

            ThemeEvents.CameraUpdated += OnCameraUpdated;

            if (m_TargetElement == null)
            {
                Debug.LogError($"[PositionToVisualElement]: Element '{m_ElementName}' not found.");
                return;
            }

            m_TargetElement.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

        }

        void OnDisable()
        {
            ThemeEvents.CameraUpdated -= OnCameraUpdated;

            if (m_TargetElement != null)
            {
                m_TargetElement.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            }
        }


        void Start()
        {
            MoveToElement();
        }

        public void MoveToElement()
        {
            if (m_Camera == null)
            {
                Debug.LogError("[PositionToVisualElement] MoveToElement: Camera is not assigned.");
                return;
            }

            if (m_ObjectToMove == null)
            {
                Debug.LogError("[PositionToVisualElement] MoveToElement: Object to move is not assigned.");
                return;
            }

            // Locate the center screen position in UI Toolkit
            Rect worldBound = m_TargetElement.worldBound;
            Vector2 centerPosition = new Vector2(worldBound.x + worldBound.width / 2, worldBound.y + worldBound.height / 2);

            // Convert to pixel coordinates using extension method
            Vector2 screenPos = centerPosition.GetScreenCoordinate(m_Document.rootVisualElement);

            // Convert to world position using extension method
            Vector3 worldPosition = screenPos.ScreenPosToWorldPos(m_Camera, m_Depth);

            if (m_ObjectToMove != null)
            {
                m_ObjectToMove.transform.position = worldPosition;
            }
        }

        // Update the Camera for Portrait/Landscape Themes
        void OnCameraUpdated(Camera camera)
        {
            m_Camera = camera;
            MoveToElement();
        }

        // Move the GameObject whenever the UI element sets up or moves
        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            MoveToElement();
        }
    }
}

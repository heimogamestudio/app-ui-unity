using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace UIToolkitDemo
{
    [RequireComponent(typeof(UnitController))]
    public class HealDropZone : MonoBehaviour
    {
        [Tooltip("Represents Health Potion drop area over each character.")] [SerializeField]
        string m_SlotID;

        [SerializeField] UIDocument m_GameScreenDocument;

        [SerializeField] Vector2 m_WorldSize = new Vector2(1.0f, 1.0f);

        [Range(1, 100)] [SerializeField] float m_PercentHealthBoost;

        VisualElement m_Slot;
        UnitController m_UnitController;
        UnitHealthBehaviour m_UnitHealth;

        int m_MaxHealth;
        int m_HealthBoost;
        Camera m_MainCamera;

        public static Action UseOnePotion;

        void OnEnable()
        {
            GameplayEvents.SlotHealed += OnSlotHealed;

            UnitController.UnitDied += OnUnitDied;
        }

        void OnDisable()
        {
            GameplayEvents.SlotHealed -= OnSlotHealed;

            UnitController.UnitDied -= OnUnitDied;
        }

        void Start()
        {
            m_MainCamera = Camera.main;

            m_UnitController = GetComponent<UnitController>();
            m_UnitHealth = m_UnitController.healthBehaviour;
            m_MaxHealth = m_UnitController.data.totalHealth;
            m_HealthBoost = (int)(m_MaxHealth * m_PercentHealthBoost / 100f);
            SetVisualElements();
        }

        void SetVisualElements()
        {
            VisualElement rootElement = m_GameScreenDocument.rootVisualElement;

            m_Slot = rootElement.Query<VisualElement>(m_SlotID);

            EnableSlot(true);

            // Register for GeometryChangedEvent to update the slot position after layout is finalized
            m_Slot.RegisterCallback<GeometryChangedEvent>(OnLayoutReady);
        }

        void EnableSlot(bool state)
        {
            if (m_Slot == null)
                return;

            m_Slot.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        void UpdateSlotPosition()
        {
            if (m_Slot == null || m_MainCamera == null || m_UnitController == null)
            {
                Debug.Log("Not updating slot position due to missing references.");
                return;
            }

            StartCoroutine(UpdateSlotPositionWithDelay());
        }

        IEnumerator UpdateSlotPositionWithDelay()
        {
            yield return new WaitForEndOfFrame();
            // Camera switches between portrait and landscape
            m_MainCamera = Camera.main;


            // Move the slot to follow the UnitController's position
            MoveToWorldPosition(m_Slot, m_UnitController.transform.position, m_WorldSize);
        }


        // // Move the slot to match world position, similar to the health bar
        void MoveToWorldPosition(VisualElement element, Vector3 worldPosition, Vector2 worldSize)
        {
            // Get the world position and size rect relative to the panel (
            Rect rect = RuntimePanelUtils.CameraTransformWorldToPanelRect(element.panel, worldPosition, worldSize,
                m_MainCamera);
            
            // Use contentRect to get accurate width and height of the VisualElement
            float elementWidth = element.contentRect.width;
            float elementHeight = element.contentRect.height;

            // Check if the element's width and height are resolved
            if (elementWidth <= 0 || elementHeight <= 0)
            {
                // Debug.LogWarning("Element dimensions not resolved, skipping position update.");
                return;
            }

            // Adjust the position so that the bottom center of the element aligns with the world position
            // Horizontally center the element and align the bottom
            element.style.left = rect.xMin - (elementWidth / 2);
            element.style.top = rect.yMin - elementHeight;
        }

        // Event-handling methods


        void OnLayoutReady(GeometryChangedEvent evt)
        {
            m_Slot.UnregisterCallback<GeometryChangedEvent>(OnLayoutReady);
            UpdateSlotPosition();
        }

        void OnSlotHealed(VisualElement activeSlot)
        {
            // healed the associated unit
            if (activeSlot == m_Slot)
            {
                m_UnitHealth.ChangeHealth(m_HealthBoost);

                UseOnePotion?.Invoke();
            }
        }

        // disable healing slots for dead units
        void OnUnitDied(UnitController deadUnit)
        {
            if (deadUnit == m_UnitController)
            {
                EnableSlot(false);
            }
        }
    }
}
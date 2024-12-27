using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// Manages the drag-and-drop functionality of healing potions. Allows users to drag a potion icon and drop
    /// it onto the closest valid HealDropZone to heal a character during gameplay.
    /// </summary>

    [RequireComponent(typeof(UIDocument))]
    public class PotionScreen : MonoBehaviour
    {
        [Tooltip("Check to visualize healing potion drop slots when dragging.")]
        [SerializeField] bool m_IsSlotVisible;

        // USS class names
        const string k_DropZoneClass = "healing-potion__slot";

        const string k_PotionIconActiveClass = "potion--active";
        const string k_PotionIconInactiveClass = "potion--inactive";

        // Game screen document
        UIDocument m_Document;

        // Draggable portion of the screen area
        VisualElement m_DragArea;

        // Element to begin dragging
        VisualElement m_StartElement;

        // Potion image that acts as pointer
        VisualElement m_PointerIcon;

        // "Dropzone" regions marked for each character
        List<VisualElement> m_HealDropZones;
        
        // The closest DropZone within the activation distance
        VisualElement m_ActiveZone;

        // Text element that displays the number of potions left
        Label m_HealPotionCount;

        // is the pointer currently active
        bool m_IsDragging;

        // is one or more potions left?
        bool m_IsPotionAvailable;

        // used to calculate offset between potion icon and mouse pointer
        Vector3 m_IconStartPosition;
        Vector3 m_PointerStartPosition;

        /// <summary>
        /// Subscribe to events.
        /// </summary>
        void OnEnable()
        {
            GameplayEvents.HealingPotionUpdated += OnHealingPotionsUpdated;
        }

        /// <summary>
        /// Unsubscribe from events.
        /// </summary>
        void OnDisable()
        {
            GameplayEvents.HealingPotionUpdated -= OnHealingPotionsUpdated;
        }
 
        /// <summary>
        /// Performs setup and initialization.
        /// </summary>
        void Awake()
        {
            if (m_Document == null)
                m_Document = GetComponent<UIDocument>();

            SetVisualElements();
            RegisterCallbacks();
            HideDragArea();
        }

        void SetVisualElements()
        {
            m_Document = GetComponent<UIDocument>();
            VisualElement rootElement = m_Document.rootVisualElement;

            m_DragArea = rootElement.Q<VisualElement>("healing-potion__drag-area"); // Interactive part of the screen
            m_StartElement = rootElement.Q<VisualElement>("healing-potion__space"); // The UI element to click and drag
            m_PointerIcon = rootElement.Q<VisualElement>("healing-potion__image");  // Cursor icon that represents the dragged potion
            m_HealPotionCount = rootElement.Q<Label>("healing-potion__count");  // Text label showing available potions
            m_HealDropZones = rootElement.Query<VisualElement>(className: k_DropZoneClass).ToList(); // List of places to drop the potion
        }

        void RegisterCallbacks()
        {
            // Listen for mouse/touch movement
            m_DragArea.RegisterCallback<PointerMoveEvent>(PointerMoveEventHandler);
            
            // Listen for mouse button/touch down
            m_StartElement.RegisterCallback<PointerDownEvent>(PointerDownEventHandler);

            // listen for mouse button/touch up
            m_DragArea.RegisterCallback<PointerUpEvent>(PointerUpEventHandler);
            
        }

        /// <summary>
        /// Handles pointer movement events and updates the potion icon position.
        /// Activates the closest HealDropZone within a threshold distance.
        /// </summary>
        /// <param name="evt">Pointer move event data.</param>
        void PointerMoveEventHandler(PointerMoveEvent evt)
        {
            if (m_IsDragging && m_DragArea.HasPointerCapture(evt.pointerId))
            {
                // Move the potion icon
                MovePotionIcon(evt.position);

                // Find the closest drop zone
                float activationDistance = 100f; // Adjust as necessary
                VisualElement closestZone = FindClosestDropZone(evt.position, activationDistance);

                // Activate the closest drop zone
                ActivateClosestDropZone(closestZone);
            }
        }
        
        /// <summary>
        /// Updates the potion icon's position based on the pointer movement.
        /// </summary>
        /// <param name="pointerPosition">Current pointer position.</param>
        private void MovePotionIcon(Vector2 pointerPosition)
        {
            float newX = m_IconStartPosition.x + (pointerPosition.x - m_PointerStartPosition.x);
            float newY = m_IconStartPosition.y + (pointerPosition.y - m_PointerStartPosition.y);

            m_PointerIcon.transform.position = new Vector2(newX, newY);
        }

        /// <summary>
        /// Finds the closest HealDropZone to the current pointer position within a given distance.
        /// </summary>
        /// <param name="pointerPosition">Current pointer position.</param>
        /// <param name="activationDistance">Distance threshold for activation.</param>
        /// <returns>Closest HealDropZone if within distance, otherwise null.</returns>
        VisualElement FindClosestDropZone(Vector2 pointerPosition, float activationDistance)
        {
            float closestDistance = float.MaxValue;
            VisualElement closestZone = null;

            foreach (VisualElement slot in m_HealDropZones)
            {
                Vector2 slotCenter = slot.worldBound.center;
                float distance = Vector2.Distance(pointerPosition, slotCenter);

                if (distance < activationDistance && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestZone = slot;
                }
            }

            return closestZone;
        }

        /// <summary>
        /// Activates the closest HealDropZone and deactivates the previous active zone, if any.
        /// </summary>
        /// <param name="closestZone">The closest HealDropZone to activate.</param>
        void ActivateClosestDropZone(VisualElement closestZone)
        {
            if (m_ActiveZone != closestZone)
            {
                // Deactivate the previous zone if it exists
                if (m_ActiveZone != null)
                {
                    m_ActiveZone.style.opacity = 0f;
                }

                // Activate the closest zone
                if (closestZone != null)
                {
                    m_ActiveZone = closestZone;
                    m_ActiveZone.style.opacity = 0.25f;
                }
                else
                {
                    m_ActiveZone = null;
                }
            }
        }

        /// <summary>
        /// Initiates dragging.
        /// </summary>
        /// <param name="evt"></param>
        void PointerDownEventHandler(PointerDownEvent evt)
        {
            if (!m_IsPotionAvailable)
                return;

            // enable the drag area and hides the slots
            m_DragArea.style.display = DisplayStyle.Flex;
            HideDropZones();

            // send all pointer events to the DragArea element
            m_DragArea.CapturePointer(evt.pointerId);

            // set the icon and pointer starting positions
            m_IconStartPosition = m_PointerIcon.transform.position;
            m_PointerStartPosition = evt.position;

            m_IsDragging = true;
        }

        /// <summary>
        /// Release the pointer.
        /// </summary>
        /// <param name="evt"></param>
        void PointerUpEventHandler(PointerUpEvent evt)
        {
            // Disable the drag area and release the pointer
            m_DragArea.style.display = DisplayStyle.None;
            m_DragArea.ReleasePointer(evt.pointerId);
            m_IsDragging = false;
            
            // Restore the potion icon
            m_PointerIcon.transform.position = m_IconStartPosition;

            // Send a message with the selected slot
            if (m_ActiveZone != null)
            {
                // Notify the GameHealDrop components and reset
                GameplayEvents.SlotHealed?.Invoke(m_ActiveZone);
                m_ActiveZone = null;
            }
        }
        
        /// <summary>
        /// Hides all HealDropZones by setting their opacity to zero.
        /// </summary>
        void HideDropZones()
        {
            foreach (VisualElement slot in m_HealDropZones)
            {
                slot.style.opacity = 0f;
            }
        }
        
        /// <summary>
        /// Hides the element representing the draggable, interactive part of the screen..
        /// </summary>
        void HideDragArea()
        {
            m_DragArea.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Updates the potion count when the number of available potions changes.
        /// </summary>
        /// <param name="potionCount">Number of available healing potions.</param>
        void OnHealingPotionsUpdated(int potionCount)
        {

            m_IsPotionAvailable = (potionCount > 0);

            EnablePotionIcon(m_IsPotionAvailable);

            m_HealPotionCount.text = potionCount.ToString();
        }

        /// <summary>
        /// Enables or disables the potion icon based on the availability of potions.
        /// </summary>
        /// <param name="state">True if potions are available, false otherwise.</param>
        void EnablePotionIcon(bool state)
        {
            if (state)
            {
                m_PointerIcon.RemoveFromClassList(k_PotionIconInactiveClass);
                m_PointerIcon.AddToClassList(k_PotionIconActiveClass);
            }
            else
            {
                m_PointerIcon.RemoveFromClassList(k_PotionIconActiveClass);
                m_PointerIcon.AddToClassList(k_PotionIconInactiveClass);
            }
        }

    }
}

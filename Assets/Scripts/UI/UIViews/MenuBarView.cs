using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    // Shows the menu buttons
    public class MenuBarView : UIView
    {
        // Classes/selectors for toggling between active and inactive states
        const string k_LabelInactiveClass = "menu__label";
        const string k_LabelActiveClass = "menu__label--active";

        const string k_IconInactiveClass = "menu__icon";
        const string k_IconActiveClass = "menu__icon--active";

        const string k_ButtonInactiveClass = "menu__button";
        const string k_ButtonActiveClass = "menu__button--active";

        // Constants for marker movement animation
        const int k_MoveTime = 150;
        const float k_Spacing = 100f;
        const float k_yOffset = -8f;

        // UI Buttons
        Button m_HomeScreenMenuButton;
        Button m_CharScreenMenuButton;
        Button m_InfoScreenMenuButton;
        Button m_ShopScreenMenuButton;
        Button m_MailScreenMenuButton;

        Button m_ActiveButton;
        bool m_InterruptAnimation;

        // Represents the currently active menu
        VisualElement m_MenuMarker;

        public MenuBarView(VisualElement topElement) : base(topElement)
        {
            // Listen for aspect ratio / viewport changes
            MediaQueryEvents.AspectRatioUpdated += OnAspectRatioUpdated;

            // Update the active menu marker if clicking the OptionsBarView's gold/gem icons
            MainMenuUIEvents.OptionsBarShopScreenShown += OnOptionsBarShopScreenShown;
        }

        public override void Dispose()
        {
            base.Dispose();

            // Unsubscribe from events
            MediaQueryEvents.AspectRatioUpdated -= OnAspectRatioUpdated;
            MainMenuUIEvents.OptionsBarShopScreenShown -= OnOptionsBarShopScreenShown;

            UnregisterButtonCallbacks();
        }

        // Set up Menu elements
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_HomeScreenMenuButton = m_TopElement.Q<Button>("menu__home-button");
            m_CharScreenMenuButton = m_TopElement.Q<Button>("menu__char-button");
            m_InfoScreenMenuButton = m_TopElement.Q<Button>("menu__info-button");
            m_ShopScreenMenuButton = m_TopElement.Q<Button>("menu__shop-button");
            m_MailScreenMenuButton = m_TopElement.Q<Button>("menu__mail-button");

            m_MenuMarker = m_TopElement.Q("menu__current-marker");
        }

        // Register callbacks for button clicks
        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();

            // Register action when each button is clicked
            m_HomeScreenMenuButton.RegisterCallback<ClickEvent>(ClickHomeButton);
            m_CharScreenMenuButton.RegisterCallback<ClickEvent>(ClickCharButton);
            m_InfoScreenMenuButton.RegisterCallback<ClickEvent>(ClickInfoButton);
            m_ShopScreenMenuButton.RegisterCallback<ClickEvent>(ClickShopButton);
            m_MailScreenMenuButton.RegisterCallback<ClickEvent>(ClickMailButton);

            // Waits for interface to build (GeometryChangedEvent), otherwise marker can miss target
            m_MenuMarker.RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
        }

        // Optional: Unregistering the button callbacks is not strictly necessary
        // in most cases and depends on your application's lifecycle management.
        // You can choose to unregister them if needed for specific scenarios.
        protected void UnregisterButtonCallbacks()
        {
            m_HomeScreenMenuButton.UnregisterCallback<ClickEvent>(ClickHomeButton);
            m_CharScreenMenuButton.UnregisterCallback<ClickEvent>(ClickCharButton);
            m_InfoScreenMenuButton.UnregisterCallback<ClickEvent>(ClickInfoButton);
            m_ShopScreenMenuButton.UnregisterCallback<ClickEvent>(ClickShopButton);
            m_MailScreenMenuButton.UnregisterCallback<ClickEvent>(ClickMailButton);

            m_MenuMarker.UnregisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
        }

        // Notify the MainMenuUIManager to show a MenuScreen. Highlight the clicked Button and
        // move the active marker to that button

        void ClickHomeButton(ClickEvent evt)
        {
            ActivateButton(m_HomeScreenMenuButton);
            MainMenuUIEvents.HomeScreenShown?.Invoke();
          
            MoveMarkerToClick(evt);
        }

        void ClickCharButton(ClickEvent evt)
        {
            ActivateButton(m_CharScreenMenuButton);
            MainMenuUIEvents.CharScreenShown?.Invoke();
            MoveMarkerToClick(evt);

        }
        void ClickInfoButton(ClickEvent evt)
        {
            ActivateButton(m_InfoScreenMenuButton);
            MainMenuUIEvents.InfoScreenShown?.Invoke();
            MoveMarkerToClick(evt);
        }

        void ClickShopButton(ClickEvent evt)
        {
            MainMenuUIEvents.ShopScreenShown?.Invoke();
            ActivateButton(m_ShopScreenMenuButton);
            MoveMarkerToClick(evt);
        }

        void ClickMailButton(ClickEvent evt)
        {
            MainMenuUIEvents.MailScreenShown?.Invoke();
            ActivateButton(m_MailScreenMenuButton);
            MoveMarkerToClick(evt);
        }

        // Activate a button, highlight its label and icon
        void ActivateButton(Button menuButton)
        {
            // Store this to refresh the marker if switching aspect ratios
            m_ActiveButton = menuButton;

            HighlightElement(menuButton, k_ButtonInactiveClass, k_ButtonActiveClass, m_TopElement);

            // Enable the label and disable others
            Label label = menuButton.Q<Label>(className: k_LabelInactiveClass);
            HighlightElement(label, k_LabelInactiveClass, k_LabelActiveClass, m_TopElement);

            // Enable the icon and disable others
            VisualElement icon = menuButton.Q<VisualElement>(className: k_IconInactiveClass);
            HighlightElement(icon, k_IconInactiveClass, k_IconActiveClass, m_TopElement);

        }

        //  Move the marker when a target VisualElement is clicked
        void MoveMarkerToClick(ClickEvent evt)
        {
            // Move the marker when we click the target VisualElement and the ClickEvent has already been processed; wait for the "BubbleUp" phase
            // to make sure the target element has fully processed the event
            if (evt.propagationPhase == PropagationPhase.BubbleUp)
            {
                MoveMarkerToElement(evt.target as VisualElement);
            }
            AudioManager.PlayDefaultButtonSound();
        }

        // Move the active marker to a target VisualElement
        void MoveMarkerToElement(VisualElement targetElement)
        {

            // world space position
            Vector2 targetInWorldSpace = targetElement.parent.LocalToWorld(targetElement.layout.position);

            // convert to local space of menu marker's parent
            Vector3 targetInRootSpace = m_MenuMarker.parent.WorldToLocal(targetInWorldSpace);

            // difference between image sizes
            Vector3 offset = new Vector3(0f, targetElement.parent.layout.height - targetElement.layout.height + k_yOffset, 0f);

            Vector3 newPosition = targetInRootSpace - offset;

            // If the aspect ratio/theme changes, use a duration of 0; otherwise, calculation based on distance
            int duration = m_InterruptAnimation ? 0 : CalculateDuration(newPosition);

            // Use the tweening tool for animation
            m_MenuMarker.experimental.animation.Position(targetInRootSpace - offset, duration);

        }

        // Calculate duration for marker animation based on distance
        int CalculateDuration(Vector3 newPosition)
        {
            // add extra animation time if moving more than one space 
            Vector3 delta = m_MenuMarker.transform.position - newPosition;

            float distanceInPixels = Mathf.Abs(delta.y / k_Spacing);

            int duration = Mathf.Clamp((int)distanceInPixels * k_MoveTime, k_MoveTime, k_MoveTime * 4);
            return duration;
        }

        // Enable the Highlight style for currently selected button/element
        void HighlightElement(VisualElement targetElement, string inactiveClass, string activeClass, VisualElement root)
        {
            if (targetElement == null)
                return;

            // Locate the currently active element 
            VisualElement currentSelection = root.Query<VisualElement>(className: activeClass);

            // If we have selected what is already currently active, do nothing
            if (currentSelection == targetElement)
            {
                return;
            }

            // De-highlight the currently active element
            currentSelection.RemoveFromClassList(activeClass);
            currentSelection.AddToClassList(inactiveClass);

            // Highlight the target element
            targetElement.RemoveFromClassList(inactiveClass);
            targetElement.AddToClassList(activeClass);
        }


        // Event-handling methods

        // When starting the scene or if switching aspect ratios
        void OnGeometryChangedEvent(GeometryChangedEvent evt)
        {

            if (m_ActiveButton == null)
                m_ActiveButton = m_HomeScreenMenuButton;

            ActivateButton(m_ActiveButton);
            MoveMarkerToElement(m_ActiveButton);
        }

        // Interrupt animation and move the marker to active button when switching aspect ratios
        void OnAspectRatioUpdated(MediaAspectRatio newAspectRatio)
        {
            m_InterruptAnimation = true;

            if (m_ActiveButton != null)
                MoveMarkerToElement(m_ActiveButton);

            m_InterruptAnimation = false;
        }

        // Open the ShopScreen from the OptionsBar. Activate the Button and move the marker without
        // a ClickEvent.
        private void OnOptionsBarShopScreenShown()
        {
            // Invoke the event to switch to the shop screen
            MainMenuUIEvents.ShopScreenShown?.Invoke();

            // Activate the corresponding menu button
            ActivateButton(m_ShopScreenMenuButton);

            // Move the marker to the 'Shop' button
            MoveMarkerToElement(m_ShopScreenMenuButton);
        }

    }
}
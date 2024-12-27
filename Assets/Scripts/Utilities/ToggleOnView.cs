using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace UIToolkitDemo
{
    [System.Serializable]
    public struct ViewState
    {
        public string viewName;
        public bool state;
    }

    /// <summary>
    ///  Utility class for toggling UI elements based on the current UI View
    /// </summary>
    public class ToggleOnView : MonoBehaviour
    {
        [Tooltip("UXML Document containing the element to toggle")]
        [SerializeField] UIDocument m_Document;
        [Tooltip("Name of the Visual Element to toggle")]
        [SerializeField] string m_ElementID;
        [Header("Enable on states")]
        [Tooltip("Specify a display state based on the name of the currently active UIView (HomeView, CharView, etc.)")]
        [SerializeField] List<ViewState> m_ViewStates = new List<ViewState>();

        VisualElement m_ElementToToggle;

        void OnEnable()
        {
            Initialize();
            MainMenuUIEvents.CurrentViewChanged += OnCurrentViewChanged;
        }

        void OnDisable()
        {
            MainMenuUIEvents.CurrentViewChanged -= OnCurrentViewChanged;
        }

        private void Initialize()
        {
            if (m_Document == null)
            {
                Debug.LogWarning("[ToggleOnMenu] UIDocument required.");
                return;
            }

            m_ElementToToggle = m_Document.rootVisualElement.Q<VisualElement>(m_ElementID);

            if (m_ElementToToggle == null)
            {
                Debug.LogWarning("[ToggleOnMenu]: Element not found.");
                return;
            }

            m_ElementToToggle.style.visibility = Visibility.Visible;
        }

        // 
        void OnCurrentViewChanged(string newViewName)
        {
            
            // Find a ViewState where the viewName == newViewName
            var matchingViewState = m_ViewStates.FirstOrDefault(x => x.viewName == newViewName);
            
            bool isMatchingView = (matchingViewState.viewName != null) ? matchingViewState.state : false;


            m_ElementToToggle.style.display = (isMatchingView) ? DisplayStyle.Flex : DisplayStyle.None;
            m_ElementToToggle.style.visibility = (isMatchingView) ? Visibility.Visible : Visibility.Hidden;


        }
    }
}


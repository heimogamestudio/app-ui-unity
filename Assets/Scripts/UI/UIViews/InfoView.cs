using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace UIToolkitDemo
{
    /// <summary>
    /// The InfoView manages a set of buttons that link to external resources. Use the ScriptableObjects in "GameData/Links"
    /// to define button details (element name, label, and URL)  in the Inspector.
    ///
    /// This allows non-programmers, such as designers, to modify these values without changing code.
    /// </summary>
    public class InfoView : UIView
    {
       
        List<Button> m_Buttons;
        List<ResourceLinkSO> m_ResourceLinks ;
        
        const string k_ResourcePath = "GameData/Links";

        /// <summary>
        /// Constructor for InfoView. Loads resource link data and sets up buttons dynamically.
        /// </summary>
        /// <param name="topElement">The root VisualElement .</param>
        public InfoView(VisualElement topElement) : base(topElement)
        {
            m_ResourceLinks = Resources.LoadAll<ResourceLinkSO>(k_ResourcePath).ToList();
            m_Buttons = new List<Button>();
            
            // Use a separate method for setting up buttons since the base.SetVisualElements
            // and base.RegisterButtonCallbacks has already run
            SetupButtons();
           
        }
        
        // Note: unregistering the button callbacks is optional and omitted in this case. Use the
        // UnregisterCallback and UnregisterValueChangedCallback methods to unregister callbacks
        // when necessary.

        /// <summary>
        /// Sets up buttons by assigning text labels and registering click events
        /// </summary>
        void SetupButtons()
        {
            // Register URL for each button
            for (int i = 0; i < m_ResourceLinks.Count; i++)
            {
                // Copy index to avoid closure in lambda expression
                int index = i; 
  
                // Use the element id in the ResourceLink to locate the button in the visual tree
                m_Buttons.Add(m_TopElement.Q<Button>(m_ResourceLinks[index].ButtonElementId));
                
                // If we find a button element, then set up its label and click event
                if (m_Buttons[index] != null)
                {
                    BindButtonLabel(m_Buttons[index], m_ResourceLinks[index]);
                    BindURL(m_Buttons[index], m_ResourceLinks[index]);
                }
            }
        }
        
        /// <summary>
        /// Binds the button's label (LocalizedText property) to the ResourceLink's ButtonText property.
        /// </summary>
        /// <param name="button">The button to bind the label to.</param>
        /// <param name="resourceLink">The ResourceLinkSO to bind the label from.</param>
        void BindButtonLabel(Button button, ResourceLinkSO resourceLink)
        {
            if (resourceLink.ButtonLabel != null)
            {
                // Assign the LocalizedString as the binding; the data source is implicit in this case, since
                // LocalizedString serves as its own data source
                button.SetBinding("text", resourceLink.ButtonLabel );
            }
        }

        /// <summary>
        /// Binds the button's click event to the TargetURL property of the ResourceLinkSO. This updates the button's
        /// clickEvent target URL when making changes in the Inspector.
        /// </summary>
        /// <param name="button">The button whose click event will be bound.</param>
        /// <param name="resourceLink">The ResourceLinkSO that provides the URL data.</param>
        void BindURL(Button button, ResourceLinkSO resourceLink)
        {
            var dataBinding = new DataBinding
            {
                dataSource = resourceLink,  // The object to bind to
                dataSourcePath = new PropertyPath(nameof(resourceLink.TargetURL)), // The specific property within the object
                bindingMode = BindingMode.ToTarget // One-way binding from data source to UI
            };
            
            // Ensure only the most recent click event is registered with the updated TargetURL
            button.UnregisterCallback<ClickEvent>(evt => OpenURL(resourceLink.TargetURL));
            button.RegisterCallback<ClickEvent>(evt => OpenURL(resourceLink.TargetURL));
            
            // Assign the binding between the target button and data source
            button.SetBinding("clickEvent", dataBinding);
            

        }
        
        /// <summary>
        /// Opens the specified URL in the default web browser and plays a click sound.
        /// </summary>
        /// <param name="URL">The URL to open.</param>
        static void OpenURL(string URL)
        {
            AudioManager.PlayDefaultButtonSound();
            Application.OpenURL(URL);
        }
    }
}

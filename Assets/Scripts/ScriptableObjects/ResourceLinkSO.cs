using UnityEngine;
using Unity.Properties;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
   /// <summary>
    /// This exposes a unique identifier, text label, and associated URL for use in UI Toolkit.
    /// This allows URL and element changes without impacting the UXML file or associated scripts.
    /// </summary>
    [CreateAssetMenu(fileName = "ResourceLinkData", menuName = "UIToolkitDemo/Resource Link")]
    public class ResourceLinkSO : ScriptableObject
    {
        // Property identifiers for change notification
        // public static readonly BindingId buttonLabelProperty = nameof(buttonLabel);
         public static readonly BindingId TargetURLProperty = nameof(TargetURL);

        // Backing fields
        [SerializeField] string m_ButtonElementId;
        [SerializeField] LocalizedString m_ButtonLabel;
        [SerializeField] string m_TargetURL;



        [Tooltip("The unique identifier for the button element in UXML")]
        public string ButtonElementId
        {
            get => m_ButtonElementId;
            set => m_ButtonElementId = value;
        }
        
        [CreateProperty]
        [Tooltip("Text label of the button")]
        public LocalizedString ButtonLabel
        {
            get => m_ButtonLabel;
            set => m_ButtonLabel = value;
        }
        
        [CreateProperty]
        [Tooltip("The URL to open in the browser")]
        public string TargetURL
        {
            get => m_TargetURL;
            set => m_TargetURL = value;
         
        }
    }
}

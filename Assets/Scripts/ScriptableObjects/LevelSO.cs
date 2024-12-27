using Unity.Properties;
using UnityEngine;
using UnityEngine.Localization;


namespace UIToolkitDemo
{
    /// <summary>
    /// holds basic level information (label name, level number, scene name for loading, thumbnail graphic for display, etc.)
    /// </summary>
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Levels/LevelData", menuName = "UIToolkitDemo/Level", order = 11)]
    public class LevelSO : ScriptableObject
    {

        [Tooltip("Background image")]
        [SerializeField] Sprite thumbnail;
        
        [Tooltip("Index number of level")]
        [SerializeField] int levelNumber;
        
        [Tooltip("Descriptive name of level")]
        [SerializeField] string levelLabel;
        
        [Tooltip("Name of the scene to load")]
        [SerializeField] string sceneName;

        
        [SerializeField] LocalizedString m_LocalizedLevelNamePrefix;
        [SerializeField] LocalizedString m_LocalizedLevelSubtitle;

        public Sprite Thumbnail => thumbnail;
        public string SceneName => sceneName;
 
        /// <summary>
        /// Returns a localized version of a level name description (e.g. "The Dragon's Lair") if it is available. Otherwise, fallback to empty string.
        /// </summary>
        [CreateProperty]
        public string LevelSubtitle
        {
            get
            {
                if (m_LocalizedLevelSubtitle != null && !string.IsNullOrEmpty(m_LocalizedLevelSubtitle.GetLocalizedString()))
                {
                    return m_LocalizedLevelSubtitle.GetLocalizedString();
                } 
                return string.Empty;  // Fallback to empty string
            }
        }

        /// <summary>
        /// Returns a localized string representing the level number (e.g. "Level 1", "Nivel 1", "Nivieu 1")
        /// </summary>
        [CreateProperty]
        public string LevelNumberFormatted
        {
            get
            {
                if (m_LocalizedLevelNamePrefix != null &&
                    !string.IsNullOrEmpty(m_LocalizedLevelNamePrefix.GetLocalizedString()))
                {
                    return m_LocalizedLevelNamePrefix.GetLocalizedString() + " " + levelNumber;
                }

                return "Level 1"; // Fallback
            }
        }
    }
}

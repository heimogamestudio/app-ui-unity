using Unity.Properties;
using UnityEngine;
using UnityEngine.Localization;

namespace UIToolkitDemo
{
    /// <summary>
    /// Enum for category of special skill (Basic, Intermediate, Advanced)
    /// </summary>
    public enum SkillCategory
    {
        Basic = 0,
        Intermediate = 1,
        Advanced = 2
    }

    /// <summary>
    /// ScriptableObject representing a character's special attack skill.
    /// Each character can have up to three special attack skills.
    /// 
    /// Note: This class uses several pre-processed string properties to localize the messages in
    /// the CharStatsView skill tab. We can simplify much of the setup using Smart Strings:
        
    /// https://docs.unity3d.com/Packages/com.unity.localization@1.5/manual/Smart/SmartStrings.html
    /// </summary>
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Skills/SkillGameData", menuName = "UIToolkitDemo/Skill",
        order = 3)]
    public class SkillSO : ScriptableObject
    {
        // The number of levels required to advance to the next tier
        const int k_LevelsPerTier = 5;

        // Backing fields
        [Tooltip("The skill name as it appears in the character stats window")] [SerializeField]
        LocalizedString m_SkillNameLocalized = new LocalizedString { TableReference = "SettingsTable" };

        [Tooltip("Basic, intermediate, and advanced skill categories.")] [SerializeField]
        SkillCategory m_Category;

        [Tooltip("Damage applied over damage time")]
        [SerializeField] int m_DamagePoints;

        [Tooltip("Time in seconds")]
        [SerializeField] float m_DamageTime;

        [Tooltip("Icon for character screen")]
        [SerializeField] Sprite m_IconSprite;

        // Skill name
        string m_LocalizedSkillName;

        // Skill Categories
        LocalizedString m_CategoryLocalized = new LocalizedString { TableReference = "SettingsTable" };
        string m_LocalizedCategoryName;
        
        // Skill tiers
        LocalizedString m_TierTextLocalized = new LocalizedString 
        { 
            TableReference = "SettingsTable",
            TableEntryReference = "Skill.TierFormat"
        };
    
        LocalizedString m_NextTierTextLocalized = new LocalizedString 
        { 
            TableReference = "SettingsTable",
            TableEntryReference = "Skill.NextTierLevelFormat"
        }; 
        string m_LocalizedTierText;
        string m_LocalizedNextTierText;
        int m_CurrentLevel;  // Store current level


        // Localized string for damage text
        LocalizedString m_DamageTextLocalized = new LocalizedString 
        { 
            TableReference = "SettingsTable",
            TableEntryReference = "Skill.DamageFormat"
        };
        string m_LocalizedDamageText;
        
        // Data-binding requires properties instead of methods. Use the CreateProperty attribute
        // to prepare properties for data-binding.

        [CreateProperty] public string SkillName => m_LocalizedSkillName;
        [CreateProperty] public int DamagePoints => m_DamagePoints;
        [CreateProperty] public float DamageTime => m_DamageTime;
        [CreateProperty] public Sprite IconSprite => m_IconSprite;
        
        /// <summary>
        /// Gets the display text for the skill's category, converting from a localized string.
        /// </summary>
        [CreateProperty]
        public string CategoryText => m_LocalizedCategoryName;
 
        // Logic for current Tier and next Tier
        int GetCurrentTier(int level) => (int)Mathf.Floor((float)level / k_LevelsPerTier) + 1;
        int GetNextTierLevel(int tier) => tier * k_LevelsPerTier;

        /// <summary>
        /// Gets the formatted, localized tier text.
        /// 
        /// English: "Tier 2"
        /// French: "Rang 2"
        /// Danish: "Rang 2"
        /// Spanish: "Rango 2"
        /// </summary>
        [CreateProperty]
        public string TierText
        {
            get
            {
                int tier = GetCurrentTier(m_CurrentLevel);
                return string.Format(m_LocalizedTierText, tier);
            }
        }

        /// <summary>
        /// Gets the formatted, localized text indicating when the next tier unlocks.
        /// 
        /// English: "Next tier at Level 5"
        /// French: "Rang suivant au niveau 5"
        /// Danish: "Næste rang ved niveau 5"
        /// Spanish: "Siguiente rango en nivel 5"
        /// </summary>
        [CreateProperty]
        public string NextTierLevelText
        {
            get
            {
                int tier = GetCurrentTier(m_CurrentLevel);
                int nextLevel = GetNextTierLevel(tier);
                return string.Format(m_LocalizedNextTierText, nextLevel);
            }
        }
        
        /// <summary>
        /// Gets the formatted, localized text describing the skill's damage over time.
        /// 
        /// English: "Deals 100 Damage points to an enemy every 2.5 seconds"
        /// French: "Inflige 100 points de dégâts à un ennemi toutes les 2,5 secondes"
        /// Danish: "Giver 100 skadepoint til en fjende hvert 2,5 sekund"
        /// Spanish: "Inflige 100 puntos de daño a un enemigo cada 2,5 segundos"
        /// </summary>
        [CreateProperty]
        public string DamageText => string.Format(m_LocalizedDamageText, DamagePoints, DamageTime);

        /// <summary>
        ///  Initializes the skill and registers for localization changes.
        /// </summary>
        void OnEnable()
        {
            // Register for localization changes
            m_CategoryLocalized.StringChanged += OnCategoryTextChanged;
            m_SkillNameLocalized.StringChanged += OnSkillNameChanged;
            m_TierTextLocalized.StringChanged += OnTierTextChanged;
            m_NextTierTextLocalized.StringChanged += OnNextTierTextChanged;
            m_DamageTextLocalized.StringChanged += OnDamageTextChanged;
        }

        /// <summary>
        /// Cleans up event handlers and unregisters.
        /// </summary>
        void OnDisable()
        {
            m_CategoryLocalized.StringChanged -= OnCategoryTextChanged;
            m_SkillNameLocalized.StringChanged -= OnSkillNameChanged;
            m_TierTextLocalized.StringChanged -= OnTierTextChanged;
            m_NextTierTextLocalized.StringChanged -= OnNextTierTextChanged;
            m_DamageTextLocalized.StringChanged -= OnDamageTextChanged;
        }

        /// <summary>
        /// Updates the localized string key for SkillCategory
        /// </summary>
        void OnValidate()
        {
            // Update the localized string key based on the category enum
            m_CategoryLocalized.TableEntryReference = $"SkillCategory.{m_Category}";
        }
        
        /// <summary>
        /// Cache the current level.
        /// </summary>
        /// <param name="level">The current character level.</param>
        public void UpdateLevel(int level)
        {
            m_CurrentLevel = level;
        }
        
        /// <summary>
        /// Update the category string when the locale changes.
        /// </summary>
        /// <param name="localizedText"></param>
        void OnCategoryTextChanged(string localizedText)
        {
            m_LocalizedCategoryName = localizedText;
        }

        /// <summary>
        /// Update the skill name when the locale changes.
        /// </summary>
        /// <param name="localizedText"></param>
        void OnSkillNameChanged(string localizedText)
        {
            m_LocalizedSkillName = localizedText;
        }

        /// <summary>
        /// Handle locale changes to update the tier text.
        /// </summary>
        /// <param name="localizedText"></param>
        void OnTierTextChanged(string localizedText)
        {
            m_LocalizedTierText = localizedText;
        }

        /// <summary>
        /// Handle locale changes to update the next tier at level text.
        /// </summary>
        /// <param name="localizedText"></param>
        void OnNextTierTextChanged(string localizedText)
        {
            m_LocalizedNextTierText = localizedText;
        }
        
        /// <summary>
        /// Handle locale changes to update the damage description text.
        /// </summary>
        void OnDamageTextChanged(string localizedText)
        {
            m_LocalizedDamageText = localizedText;
        }
        
    }
}
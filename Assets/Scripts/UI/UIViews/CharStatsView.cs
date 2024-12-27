using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// Manages the character statistics portion of the CharView screen
    /// </summary>
    public class CharStatsView : UIView
    {
        // Constants 
        const float k_ClickCooldown = 0.2f;

        // Next time available to click a button
        float m_TimeToNextClick = 0f;

        // Pairs icons with specific data types
        GameIconsSO m_GameIconsData;

        // Data associated with each character: inventory, xp, and base data
        CharacterData m_CharacterData;

        // Static base data from ScriptableObject
        CharacterBaseSO m_BaseStats;

        // Stats tab
        Label m_LevelLabel;

        // Skills tab
        VisualElement m_ActiveFrame;
        int m_ActiveIndex;

        VisualElement[] m_SkillIcons = new VisualElement[3];
        SkillSO[] m_BaseSkills = new SkillSO[3];

        // Elements for Character Data tab
        Label m_CharacterClass;
        Label m_Rarity;
        Label m_AttackType;

        Button m_NextSkillButton;
        Button m_LastSkillButton;

        // Elements for Bio tab
        Label m_BioTitle;

        // Setup and lifecycle methods

        /// <summary>
        /// Initializes a new instance of the CharStatsView with the specified top UI element.
        /// </summary>
        /// <param name="topElement">The root visual element of the UI.</param>
        public CharStatsView(VisualElement topElement) : base(topElement)
        {
        }

        /// <summary>
        /// Unregisters any callbacks.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            UnregisterCallbacks();
        }

        /// <summary>
        /// Queries and initializes all required visual elements.
        /// </summary>
        protected override void SetVisualElements()
        {
            m_LevelLabel = m_TopElement.Q<Label>("char-stats__level-number");
            
            m_CharacterClass = m_TopElement.Q<Label>("char-stats__class-label");
            m_Rarity = m_TopElement.Q<Label>("char-stats__rarity-label");
            m_AttackType = m_TopElement.Q<Label>("char-stats__attack-type-label");

            m_SkillIcons[0] = m_TopElement.Q("char-skills__icon1");
            m_SkillIcons[1] = m_TopElement.Q("char-skills__icon2");
            m_SkillIcons[2] = m_TopElement.Q("char-skills__icon3");
            
            m_NextSkillButton = m_TopElement.Q<Button>("char-skills__next-button");
            m_LastSkillButton = m_TopElement.Q<Button>("char-skills__last-button");

            m_BioTitle = m_TopElement.Q<Label>("char-bio__title");

            m_ActiveFrame = m_TopElement.Q("char-skills__active");

            // Ensure m_GameIconsData is loaded before bindings

            m_GameIconsData = Resources.Load("GameData/GameIcons") as GameIconsSO;
            if (m_GameIconsData == null)
            {
                Debug.LogError("[CharStatsView] SetVisualElements: Failed to load GameIconsSO from Resources.");
            }
        }

        /// <summary>
        /// Registers event callbacks for interactive UI elements.
        /// </summary>
        protected override void RegisterButtonCallbacks()
        {
            // Set up the click events
            m_NextSkillButton.RegisterCallback<ClickEvent>(SelectNextSkill);
            m_LastSkillButton.RegisterCallback<ClickEvent>(SelectLastSkill);
            m_SkillIcons[0].RegisterCallback<ClickEvent>(SelectSkill);
            m_SkillIcons[1].RegisterCallback<ClickEvent>(SelectSkill);
            m_SkillIcons[2].RegisterCallback<ClickEvent>(SelectSkill);

            // Initialize the active frame position after the layout builds
            m_SkillIcons[0].RegisterCallback<GeometryChangedEvent>(InitializeSkillMarker);
        }
        
        // Update methods / data binding

        /// <summary>
        /// Updates the window when switching characters. Called externally by CharView class.
        /// </summary>
        /// <param name="charData">The character data to display.</param>
        public void UpdateCharacterStats(CharacterData charData)
        {
            m_CharacterData = charData;

            // Cache skills
            m_BaseStats = charData.CharacterBaseData;
            m_BaseSkills[0] = m_BaseStats.Skill1;
            m_BaseSkills[1] = m_BaseStats.Skill2;
            m_BaseSkills[2] = m_BaseStats.Skill3;

            // Update all skills with current level
            foreach (var skill in m_BaseSkills)
            {
                if (skill != null)
                {
                    skill.UpdateLevel(charData.CurrentLevel);
                }
            }

            // Set the data source for character stats and bio
            SetCharDataSource();
            
            // Re-bind the character and bio data (first and third tabs)
            BindCharacterData();

            // Reset selected skill
            UpdateSkillView(0);
        }

        /// <summary>
        /// Sets the data source for the character stats and bio elements.
        /// </summary>
        void SetCharDataSource()
        {
            // Ensure the character data is used as the main data source
            m_LevelLabel.dataSource = m_CharacterData;
            
            // Set dataSource for character stats
            VisualElement statsContainer = m_TopElement.Q<VisualElement>("char-data-stats-content");
            statsContainer.dataSource = m_BaseStats;

            // Set dataSources for the bio and title
            VisualElement bioContainer = m_TopElement.Q<VisualElement>("char-data-bio-content");
            bioContainer.dataSource = m_BaseStats;

            // Update the character icons
            m_GameIconsData.UpdateIcons(m_BaseStats.CharacterClass, m_BaseStats.Rarity, m_BaseStats.AttackType);

            // Set the data source for the skill icons
            m_SkillIcons[0].dataSource = m_BaseStats;
            m_SkillIcons[1].dataSource = m_BaseStats;
            m_SkillIcons[2].dataSource = m_BaseStats;
        }

        /// <summary>
        /// Sets the data source for the skill elements.
        /// </summary>
        /// <param name="newSkillDataSource"></param>
        void SetSkillDataSources(SkillSO newSkillDataSource)
        {
             VisualElement skillContainer = m_TopElement.Q<VisualElement>("char-data-skills-content");
             skillContainer.dataSource = newSkillDataSource;
        }

        /// <summary>
        /// Updates/rebinds special LocalizedStrings that require pre-processing. 
        /// These LocalizedStrings are derived from enums and converted to strings before localization.
        /// </summary>
        void BindCharacterData()
        {
            if (m_BaseStats == null)
            {
                Debug.LogWarning("[CharStatsView] BindCharacterData: m_BaseStats is null");
                return;
            }
            
            // Binds LocalizedStrings to UI elements that require pre-processing from enum values
            m_CharacterClass.SetBinding("text", m_BaseStats.CharacterClassLocalized);
            m_Rarity.SetBinding("text", m_BaseStats.RarityLocalized);
            m_AttackType.SetBinding("text", m_BaseStats.AttackTypeLocalized);
            m_BioTitle.SetBinding("text", m_BaseStats.BioTitleLocalized);
        }

        /// <summary>
        /// Unregisters event callbacks from interactive UI elements to prevent memory leaks.
        /// </summary>
        void UnregisterCallbacks()
        {
            m_NextSkillButton.UnregisterCallback<ClickEvent>(SelectNextSkill);
            m_LastSkillButton.UnregisterCallback<ClickEvent>(SelectLastSkill);
            m_SkillIcons[0].UnregisterCallback<ClickEvent>(SelectSkill);
            m_SkillIcons[1].UnregisterCallback<ClickEvent>(SelectSkill);
            m_SkillIcons[2].UnregisterCallback<ClickEvent>(SelectSkill);

            // Initialize the active frame position after the layout builds
            m_SkillIcons[0].UnregisterCallback<GeometryChangedEvent>(InitializeSkillMarker);
        }

        /// <summary>
        /// Handles the click event to select the previous skill from buttons.
        /// </summary>
        /// <param name="evt"></param>
        void SelectLastSkill(ClickEvent evt)
        {
            if (Time.time < m_TimeToNextClick)
                return;

            m_TimeToNextClick = Time.time + k_ClickCooldown;

            // Only select when clicking directly on the visual element
            m_ActiveIndex--;
            if (m_ActiveIndex < 0)
            {
                m_ActiveIndex = 2;
            }

            UpdateSkillView(m_ActiveIndex);
            AudioManager.PlayDefaultButtonSound();
        }

        /// <summary>
        /// Handles the click event to select the next skill from buttons.
        /// </summary>
        /// <param name="evt"></param>
        void SelectNextSkill(ClickEvent evt)
        {
            if (Time.time < m_TimeToNextClick)
                return;

            m_TimeToNextClick = Time.time + k_ClickCooldown;
            m_ActiveIndex++;

            if (m_ActiveIndex > 2)
            {
                m_ActiveIndex = 0;
            }

            UpdateSkillView(m_ActiveIndex);
            AudioManager.PlayDefaultButtonSound();
        }

        /// <summary>
        /// Determines which skill icon was clicked based on the clicked element's name.
        /// </summary>
        /// <param name="evt">The click event for the skill button.</param>
        void SelectSkill(ClickEvent evt)
        {
            VisualElement clickedElement = evt.target as VisualElement;

            if (clickedElement == null)
                return;

            // Get the last character from the clicked element's name (e.g. char-skills__icon1, char-skills__icon2, etc.)
            // and convert that into an integer
            int index = (int)Char.GetNumericValue(clickedElement.name[clickedElement.name.Length - 1]) - 1;
            index = Mathf.Clamp(index, 0, m_BaseSkills.Length - 1);

            // Show the corresponding ScriptableObject data
            UpdateSkillView(index);

            // Play a click sound
            AudioManager.PlayAltButtonSound();
        }

        /// <summary>
        /// Updates the UI to display information about the skill at the specified index.
        /// </summary>
        /// <param name="index">The index of the skill to display.</param>
        void UpdateSkillView(int index)
        {
            // Abort without valid character data
            if (m_CharacterData == null || m_BaseSkills == null || m_BaseSkills[index] == null)
                return;
            
            SkillSO newSkillDataSource = m_BaseSkills[index];

            // Update the skill's level before setting it as data source
            newSkillDataSource.UpdateLevel(m_CharacterData.CurrentLevel);
            
            // Update the data source to use the currently selected skill
            SetSkillDataSources(newSkillDataSource);

            // Highlight the selected skill icon
            MarkTargetElement(m_SkillIcons[index], 300);

            // Track the new active index
            m_ActiveIndex = index;
        }

        /// <summary>
        /// Initializes the active skill marker's position after the UI layout has been built.
        /// </summary>
        /// <param name="evt">The geometry changed event data.</param>
        // set up the active frame after the layout builds
        void InitializeSkillMarker(GeometryChangedEvent evt)
        {
            // set its position over the first icon
            MarkTargetElement(m_SkillIcons[0], 0);
        }

        /// <summary>
        /// Animates the active frame to highlight the specified target element.
        /// </summary>
        /// <param name="targetElement">The UI element to highlight.</param>
        /// <param name="duration">The animation duration in milliseconds.</param>
        void MarkTargetElement(VisualElement targetElement, int duration = 200)
        {
            // target element, converted into the root space of the Active Frame
            Vector3 targetInRootSpace = GetRelativePosition(targetElement, m_ActiveFrame);

            // padding offset
            Vector3 offset = new Vector3(10, 10, 0f);

            m_ActiveFrame.experimental.animation.Position(targetInRootSpace - offset, duration);
        }

        /// <summary>
        /// Converts the position of an element into the coordinate space of another element's parent.
        /// </summary>
        /// <param name="elementToConvert">The element to convert.</param>
        /// <param name="targetElement">The target element.</param>
        /// <returns>The position of the target element in the new root space.</returns>
        static Vector3 GetRelativePosition(VisualElement elementToConvert, VisualElement targetElement)
        {
            Vector2 worldSpacePosition = elementToConvert.parent.LocalToWorld(elementToConvert.layout.position);
            VisualElement newRoot = targetElement.parent;
            return newRoot.WorldToLocal(worldSpacePosition);
        }
    }
}
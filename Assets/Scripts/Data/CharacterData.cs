using System;
using Unity.Properties;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    // Stores data for character instance + static data from a ScriptableObject

    /// <summary>
    /// Manages the character's dynamic data, including equipped gear and current level.
    /// Uses runtime data binding system to notify UI elements of property changes.
    /// Calculates XP requirements, potions needed for the next level, and current power.
    /// </summary>
    public class CharacterData : MonoBehaviour, INotifyBindablePropertyChanged
    {
        // How quickly XP requirements increase as level increases
        const float k_ProgressionFactor = 10f;

        /// <summary>
        /// Gets the array of equipped gear items for the character.
        /// </summary>
        [SerializeField] EquipmentSO[] m_GearData = new EquipmentSO[4];

        /// <summary>
        /// Gets the static data for the character from a ScriptableObject.
        /// </summary>
        [SerializeField] CharacterBaseSO m_CharacterBaseData;

        [SerializeField] int m_CurrentLevel;

        /// <summary>
        /// Gets or sets the current level of the character, and notifies the binding system on change.
        /// </summary>
        [CreateProperty]
        public int CurrentLevel
        {
            get => m_CurrentLevel;
            set
            {
                if (m_CurrentLevel == value)
                    return;

                m_CurrentLevel = value;

                Notify();

                // Notify that CurrentPower and PotionsForNextLevel also need updates when CurrentLevel changes.
                Notify(nameof(CurrentPower));
                Notify(nameof(PotionsForNextLevel));
            }
        }

        /// <summary>
        /// Calculates the current power level of the character, based on level and base stats. Updated
        /// when CurrentLevel changes.
        /// </summary>
        [CreateProperty]
        public int CurrentPower
        {
            get
            {
                float basePoints = m_CharacterBaseData.TotalBasePoints;

                // Add logic here optionally to include the character's gear
                float equipmentPoints = 0f;

                return (int)(CurrentLevel * basePoints + equipmentPoints) / 10;
            }
        }

        /// <summary>
        /// Gets the character name from CharacterBaseData.
        /// </summary>
        [CreateProperty]
        public string CharacterName => m_CharacterBaseData.CharacterName;

        /// <summary>
        /// Calculates the number of potions needed for the next level and returns a formatted string.
        /// </summary>
        [CreateProperty]
        public string PotionsForNextLevel => "/" + GetPotionsForNextLevel(CurrentLevel, k_ProgressionFactor);

        /// <summary>
        /// Event raised when a bindable property changes (required for INotifyBindablePropertyChanged)
        /// </summary>
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        /// <summary>
        /// Notifies listeners that a property has changed. 
        /// </summary>
        /// <param name="property">The name of the property that has changed</param>
        void Notify([CallerMemberName] string property = "")
        {
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Gets or sets the preview instance for the character's visual representation.
        /// </summary>
        public GameObject PreviewInstance { get; set; }

        /// <summary>
        /// Gets the static data for the character from a ScriptableObject.
        /// </summary>
        public CharacterBaseSO CharacterBaseData => m_CharacterBaseData;

        /// <summary>
        /// Gets the array of equipped gear items for the character.
        /// </summary>
        public EquipmentSO[] GearData => m_GearData;

        /// <summary>
        /// Calculates the level-up potions required to reach the next level as unsigned int.
        /// </summary>
        /// <returns>The amount of XP required for the next level.</returns>
        public uint GetPotionsForNextLevel()
        {
            return (uint)GetPotionsForNextLevel(m_CurrentLevel, k_ProgressionFactor);
        }

        /// <summary>
        /// Calculates the number of potions needed to increment the character's level.
        /// </summary>
        /// <param name="currentLevel">The current level of the character.</param>
        /// <param name="progressionFactor">The progression factor used for calculation.</param>
        /// <returns>The number of potions needed to reach the next level.</returns>
        int GetPotionsForNextLevel(int currentLevel, float progressionFactor)
        {
            currentLevel = Mathf.Clamp(currentLevel, 1, currentLevel);
            progressionFactor = Mathf.Clamp(progressionFactor, 1f, progressionFactor);

            float xp = (progressionFactor * (currentLevel));
            xp = Mathf.Ceil((float)xp);
            return (int)xp;
        }

        /// <summary>
        /// Increments the character's level by 1.
        /// </summary>
        public void IncrementLevel()
        {
            CurrentLevel++;

            // Notify other systems (CharScreenController, etc.) that this character has leveled up
            CharEvents.LevelIncremented?.Invoke(this);
        }
    }
}
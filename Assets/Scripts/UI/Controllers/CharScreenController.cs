using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace UIToolkitDemo
{
    /// <summary>
    /// Controls the logic for managing the character screen, including character previews, gear management, and leveling up.
    /// Handles character selection, inventory interactions, and event-driven updates to the UI.
    /// </summary>

    public class CharScreenController : MonoBehaviour
    {
        [Tooltip("Characters to choose from.")]
        [SerializeField] List<CharacterData> m_Characters;

        [Tooltip("Parent transform for all character previews.")]
        [SerializeField] Transform m_PreviewTransform;

        [Header("Inventory")]
        [Tooltip("Check this option to allow only one type of gear (armor, weapon, etc.) per character.")]
        [SerializeField] bool m_UnequipDuplicateGearType;

        [Header("Level Up")]
        [SerializeField] [Tooltip("Controls playback of level up FX.")] PlayableDirector m_LevelUpPlayable;

        public CharacterData CurrentCharacter { get => m_Characters[m_CurrentIndex]; }

        int m_CurrentIndex;
        int m_ActiveGearSlot;

        LevelMeterData m_LevelMeterData;

        /// <summary>
        /// Registers event callbacks when the screen is enabled.
        /// </summary>
        void OnEnable()
        {
            CharEvents.LevelIncremented += OnLevelIncremented;

            CharEvents.ScreenStarted += OnCharScreenStarted;
            CharEvents.ScreenEnded += OnCharScreenEnded;
            CharEvents.NextCharacterSelected += SelectNextCharacter;
            CharEvents.LastCharacterSelected += SelectLastCharacter;
            CharEvents.InventoryOpened += OnInventoryOpened;
            CharEvents.GearAutoEquipped += OnGearAutoEquipped;
            CharEvents.GearAllUnequipped += OnGearUnequipped;
            CharEvents.LevelUpClicked += OnLevelUpClicked;
            CharEvents.CharacterLeveledUp += OnCharacterLeveledUp;

            InventoryEvents.GearSelected += OnGearSelected;
            InventoryEvents.GearAutoEquipped += OnGearAutoEquipped;

            SettingsEvents.PlayerLevelReset += OnResetPlayerLevel;
        }

        /// <summary>
        /// Unregisters event callbacks when the screen is disabled to prevent memory leaks.
        /// </summary>
        void OnDisable()
        {
            CharEvents.LevelIncremented -= OnLevelIncremented;

            CharEvents.ScreenStarted -= OnCharScreenStarted;
            CharEvents.ScreenEnded -= OnCharScreenEnded;

            CharEvents.NextCharacterSelected -= SelectNextCharacter;
            CharEvents.LastCharacterSelected -= SelectLastCharacter;
            CharEvents.InventoryOpened -= OnInventoryOpened;
            CharEvents.GearAutoEquipped -= OnGearAutoEquipped;
            CharEvents.GearAllUnequipped -= OnGearUnequipped;
            CharEvents.LevelUpClicked -= OnLevelUpClicked;
            CharEvents.CharacterLeveledUp -= OnCharacterLeveledUp;

            InventoryEvents.GearSelected -= OnGearSelected;
            InventoryEvents.GearAutoEquipped -= OnGearAutoEquipped;

            SettingsEvents.PlayerLevelReset -= OnResetPlayerLevel;
        }

        /// <summary>
        /// Initializes character previews by instantiating visual representations for each character.
        /// </summary>
        void Awake()
        {
            InitializeCharPreview();
            SetupLevelMeter();
        }

        void Start()
        {
            // Notify InventoryScreenController
            CharEvents.GearDataInitialized?.Invoke(m_Characters);
        }

        /// <summary>
        /// Sets up the initial gear data for each character and notifies the inventory system.
        /// </summary>
        void UpdateView()
        {
            if (m_Characters.Count == 0)
                return;

            // show the Character Prefab
            CharEvents.CharacterShown?.Invoke(CurrentCharacter);

            // update the four gear slots
            UpdateGearSlots();
        }

        // character preview methods
        public void SelectNextCharacter()
        {
            if (m_Characters.Count == 0)
                return;

            ShowCharacterPreview(false);

            m_CurrentIndex++;
            if (m_CurrentIndex >= m_Characters.Count)
                m_CurrentIndex = 0;

            // select next character from m_Characters and refresh the CharScreen
            UpdateView();
        }

        public void SelectLastCharacter()
        {
            if (m_Characters.Count == 0)
                return;

            ShowCharacterPreview(false);

            m_CurrentIndex--;
            if (m_CurrentIndex < 0)
                m_CurrentIndex = m_Characters.Count - 1;

            // select last character from m_Characters and refresh the CharScreen
            UpdateView();
        }

        /// <summary>
        /// Initializes character previews.
        /// </summary>
        void InitializeCharPreview()
        {
            foreach (CharacterData charData in m_Characters)
            {
                if (charData == null)
                {
                    Debug.LogWarning("[CharScreenController] InitializeCharPreview Warning: Missing character data.");
                    continue;
                }
                GameObject previewInstance = Instantiate(charData.CharacterBaseData.CharacterVisualsPrefab, m_PreviewTransform);
                
                previewInstance.transform.localPosition = Vector3.zero;
                previewInstance.transform.localRotation = Quaternion.identity;
                previewInstance.transform.localScale = Vector3.one;
                charData.PreviewInstance = previewInstance;
                previewInstance.gameObject.SetActive(false);
            }

            CharEvents.PreviewInitialized?.Invoke();
       
        }

        /// <summary>
        /// Displays the preview for the currently selected character.
        /// </summary>
        void ShowCharacterPreview(bool state)
        {
            if (m_Characters.Count == 0)
                return;

            CharacterData currentCharacter = m_Characters[m_CurrentIndex];
            currentCharacter.PreviewInstance.gameObject.SetActive(state);
        }
        

        /// <summary>
        /// Updates the character's gear slots on the screen.
        /// </summary>
        void UpdateGearSlots()
        {
            if (CurrentCharacter == null)
                return;

            for (int i = 0; i < CurrentCharacter.GearData.Length; i++)
            {
                CharEvents.GearSlotUpdated?.Invoke(CurrentCharacter.GearData[i], i);
            }
        }

        /// <summary>
        /// Removes a specific EquipmentType (helmet, shield/armor, weapon, gloves, boots) from a character;
        /// use this to prevent duplicate gear types from appearing in the inventory.
        /// </summary>
        /// <param name="typeToRemove">EquipmentType to remove.</param>
        public void RemoveGearType(EquipmentType typeToRemove)
        {
            if (CurrentCharacter == null)
                return;

            // Remove type from each character's inventory slot if found and notify the CharView
            for (int i = 0; i < CurrentCharacter.GearData.Length; i++)
            {
                if (CurrentCharacter.GearData[i] != null && CurrentCharacter.GearData[i].equipmentType == typeToRemove)
                {
                    CharEvents.GearItemUnequipped(CurrentCharacter.GearData[i]);
                    
                    CurrentCharacter.GearData[i] = null;
                    
                    CharEvents.GearSlotUpdated?.Invoke(null, i);
                }
            }
        }

        // Character level methods
        
        /// <summary>
        /// Calculates and returns the total levels of all characters.
        /// </summary>
        int GetTotalLevels()
        {
            int totalLevels = 0;
            foreach (CharacterData charData in m_Characters)
            {
                totalLevels += charData.CurrentLevel;
            }
            return totalLevels;
        }

        /// <summary>
        /// Sets up the level meter data, tracking the total levels of all characters.
        /// </summary>
        void SetupLevelMeter()
        {
            m_LevelMeterData = new LevelMeterData(GetTotalLevels());
            
            CharEvents.GetLevelMeterData = () => m_LevelMeterData;
        }
        
        /// <summary>
        /// Updates the level meter to reflect any changes in the total character levels.
        /// </summary>
        void UpdateLevelMeter()
        {
            m_LevelMeterData.TotalLevels = GetTotalLevels();
        }

        // Event-handling methods

        /// <summary>
        /// Resets the level of all characters and updates the screen to reflect the changes.
        /// </summary>
        void OnResetPlayerLevel()
        {
            foreach (CharacterData charData in m_Characters)
            {
                charData.CurrentLevel = 0;
            }
            CharEvents.CharacterShown?.Invoke(CurrentCharacter);
            UpdateLevelMeter();
        }
        
        void OnCharScreenStarted()
        {
            UpdateView();
            ShowCharacterPreview(true);
        }

        void OnCharScreenEnded()
        {
            ShowCharacterPreview(false);
        }

        // click the level up button
        void OnLevelUpClicked()
        {
            // notify GameDataManager that we want to spend LevelUpPotion
            CharEvents.LevelPotionUsed?.Invoke(CurrentCharacter);
        }

        // update the character stats UI
        void OnLevelIncremented(CharacterData charData)
        {
            if (charData == CurrentCharacter)
            {
                CharEvents.CharacterShown?.Invoke(CurrentCharacter);
                UpdateLevelMeter();
            }
        }

        /// <summary>
        /// Handles the result of a character leveling up, incrementing their level and playing visual effects.
        /// </summary>
        /// <param name="didLevel">Was leveling up the character successful.</param>
        // 
        void OnCharacterLeveledUp(bool didLevel)
        {
            if (didLevel)
            {
                CurrentCharacter.IncrementLevel();

                // Play back the FX sequence
                m_LevelUpPlayable.Play();
            }
        }

        /// <summary>
        /// Track the gear slot used to open the Inventory
        /// </summary>
        /// <param name="gearSlot">The index of the gear slot.</param>
        void OnInventoryOpened(int gearSlot)
        {
            m_ActiveGearSlot = gearSlot;
        }

        /// <summary>
        /// Equips selected gear to the active gear slot and updates the UI.
        /// </summary>
        /// <param name="gearObject">The selected gear/equipment item.</param>
        // Handles gear selection from the Inventory Screen
        void OnGearSelected(EquipmentSO gearObject)
        {
            // If Slot already has an item, notify the InventoryScreenController and return it to the inventory
            if (CurrentCharacter.GearData[m_ActiveGearSlot] != null)
            {

                CharEvents.GearItemUnequipped?.Invoke(CurrentCharacter.GearData[m_ActiveGearSlot]);
                CurrentCharacter.GearData[m_ActiveGearSlot] = null;
            }

            // Remove any duplicate EquipmentTypes (only permit one type of helmet, shield/armor, weapon, gloves, or boots)
            if (m_UnequipDuplicateGearType)
            {
                RemoveGearType(gearObject.equipmentType);
            }

            // Set the Gear into the active slot
            CurrentCharacter.GearData[m_ActiveGearSlot] = gearObject;

            // Notify CharScreen to update
            CharEvents.GearSlotUpdated?.Invoke(gearObject, m_ActiveGearSlot);
        }

        /// <summary>
        /// Unequip all gear slots.
        /// </summary>
        void OnGearUnequipped()
        {
            for (int i = 0; i < CurrentCharacter.GearData.Length; i++)
            {
                // If we currently have Equipment in one of the four gear slots, remove it
                if (CurrentCharacter.GearData[i] != null)
                {
                    // Notifies the InventoryScreenController to unequip gear and update lists
                    CharEvents.GearItemUnequipped?.Invoke(CurrentCharacter.GearData[i]);

                    // Clear the Equipment from the character's gear data
                    CurrentCharacter.GearData[i] = null;

                    // Notify the CharScreen UI to update
                    CharEvents.GearSlotUpdated?.Invoke(null, i);
                }
            }
        }

        /// <summary>
        /// Auto-equips gear for the current character and updates the UI.
        /// </summary>
        void OnGearAutoEquipped()
        {
            CharEvents.CharacterAutoEquipped?.Invoke(CurrentCharacter);
        }

        /// <summary>
        /// Automatically equips gear to the character's empty gear slots and updates the screen.
        /// </summary>
        void OnGearAutoEquipped(List<EquipmentSO> gearToEquip)
        {
            if (CurrentCharacter == null)
                return;

            int gearCounter = 0;

            for (int i = 0; i < CurrentCharacter.GearData.Length; i++)
            {
                if (CurrentCharacter.GearData[i] == null && gearCounter < gearToEquip.Count)
                {
                    CurrentCharacter.GearData[i] = gearToEquip[gearCounter];

                    // notify the CharView to update
                    CharEvents.GearSlotUpdated?.Invoke(gearToEquip[gearCounter], i);
                    gearCounter++;
                }
            }
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace UIToolkitDemo
{
    /// <summary>
    /// Manages the character view UI, including leveling up buttons, gear slots, and character stats.
    /// Uses a mix of event-driven updates (inventory) and runtime data binding (potion labels, character name, power).
    /// </summary>
    public class CharView : UIView
    {
        const string k_LevelUpButtonInactiveClass = "footer__level-up-button--inactive";
        const string k_LevelUpButtonClass = "footer__level-up-button";

        readonly Button[] m_GearSlots = new Button[4];
        readonly Sprite m_EmptyGearSlotSprite;

        Button m_LastCharButton;
        Button m_NextCharButton;
        Button m_AutoEquipButton;
        Button m_UnequipButton;
        Button m_LevelUpButton;

        Label m_CharacterLabel;
        Label m_PotionsForNextLevel;
        Label m_PotionCount;
        Label m_PowerLabel;

        VisualElement m_LevelUpButtonVFX;

        CharStatsView m_CharStatsView; // Window that displays character stats

        /// <summary>
        /// Initializes the character view with the specified top-level UI element.
        /// </summary>
        /// <param name="topElement">The root visual element of the UI.</param>
        public CharView(VisualElement topElement) : base(topElement)
        {
            CharEvents.LevelUpButtonEnabled += OnLevelUpButtonEnabled;
            CharEvents.CharacterShown += OnCharacterUpdated;
            CharEvents.PreviewInitialized += OnInitialized;
            CharEvents.GearSlotUpdated += OnGearSlotUpdated;

            GameDataManager.GameDataReceived += OnGameDataReceived;

            GameDataManager.GameDataRequested?.Invoke();

            // Locate the empty gear slot sprite from a ScriptableObject icons
            var gameIconsData = Resources.Load("GameData/GameIcons") as GameIconsSO;
            m_EmptyGearSlotSprite = gameIconsData.emptyGearSlotIcon;

            m_CharStatsView = new CharStatsView(topElement.Q<VisualElement>("CharStatsWindow"));
            m_CharStatsView.Show();
        }

        /// <summary>
        /// Called when GameData is received from GameDataManager.
        /// Applies the runtime data binding.
        /// </summary>
        /// <param name="gameData">The GameData object to bind.</param>
        void OnGameDataReceived(GameData gameData)
        {
            BindGameDataToUI(gameData);
        }

        /// <summary>
        /// Add bindings to labels for the potion count, potion for next level, character power, and character name 
        /// </summary>
        /// <param name="gameData"></param>
        void BindGameDataToUI(GameData gameData)
        {
            // Binding for Character Power label
            m_PowerLabel.SetBinding("text", new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(CharacterData.CurrentPower)),
                bindingMode = BindingMode.ToTarget
            });

            // Binding for Character Name label
            m_CharacterLabel.SetBinding("text", new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(CharacterData.CharacterName)),
                bindingMode = BindingMode.ToTarget
            });

            // Binding for Potion Count label
            var potionBinding = new DataBinding()
            {
                dataSource = gameData,
                dataSourcePath = new PropertyPath(string.Empty), // No direct path -- using a converter
                bindingMode = BindingMode.ToTarget
            };

            // Format the string label ( number of potions available / number of potions needed to update)
            potionBinding.sourceToUiConverters.AddConverter((ref GameData data) =>
                FormatPotionCountLabel(data.LevelUpPotions));
            m_PotionCount.SetBinding("text", potionBinding);

            // Binding for PotionsForNextLevel label 
            m_PotionsForNextLevel.SetBinding("text", new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(CharacterData.PotionsForNextLevel)),
                bindingMode = BindingMode.ToTarget
            });
        }

        /// <summary>
        /// Formats the potion count label with appropriate color based on potion availability.
        /// </summary>
        /// <param name="potionCount">The current potion count.</param>
        /// <returns>Formatted potion count as a string.</returns>
        string FormatPotionCountLabel(uint potionCount)
        {
            if (m_PotionsForNextLevel == null)
            {
                Debug.LogWarning("[CharView] FormatPotionCountLabel: PotionsForNextLevel label is not set.");
                return potionCount.ToString();
            }

            string potionsForNextLevelString = m_PotionsForNextLevel.text.TrimStart('/');

            if (!string.IsNullOrEmpty(potionsForNextLevelString) &&
                int.TryParse(potionsForNextLevelString, out int potionsForNextLevel))
            {
                int potionsCount = (int)potionCount;

                // Update the color of the potion count label based on comparison
                m_PotionCount.style.color = (potionsForNextLevel > potionsCount)
                    ? new Color(0.88f, 0.36f, 0f) // Orange if potions are insufficient
                    : new Color(0.81f, 0.94f, 0.48f); // Green if potions are sufficient
            }

            return potionCount.ToString();
        }

        /// <summary>
        /// Dispose of event handlers and clean up resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            CharEvents.LevelUpButtonEnabled -= OnLevelUpButtonEnabled;
            CharEvents.CharacterShown -= OnCharacterUpdated;
            CharEvents.PreviewInitialized -= OnInitialized;
            CharEvents.GearSlotUpdated -= OnGearSlotUpdated;

            GameDataManager.GameDataReceived -= OnGameDataReceived;

            UnregisterButtonCallbacks();
        }

        /// <summary>
        /// Set up references to Visual Elements in the UI.
        /// </summary>
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_GearSlots[0] = m_TopElement.Q<Button>("char-inventory__slot1");
            m_GearSlots[1] = m_TopElement.Q<Button>("char-inventory__slot2");
            m_GearSlots[2] = m_TopElement.Q<Button>("char-inventory__slot3");
            m_GearSlots[3] = m_TopElement.Q<Button>("char-inventory__slot4");

            m_NextCharButton = m_TopElement.Q<Button>("char__next-button");
            m_LastCharButton = m_TopElement.Q<Button>("char__last-button");

            m_AutoEquipButton = m_TopElement.Q<Button>("char__auto-equip-button");
            m_UnequipButton = m_TopElement.Q<Button>("char__unequip-button");
            m_LevelUpButton = m_TopElement.Q<Button>("char__level-up-button");
            m_LevelUpButtonVFX = m_TopElement.Q<VisualElement>("char__level-up-button-vfx");

            m_CharacterLabel = m_TopElement.Q<Label>("char__label");
            m_PotionCount = m_TopElement.Q<Label>("char__potion-count");
            m_PotionsForNextLevel = m_TopElement.Q<Label>("char__potion-to-advance");
            m_PowerLabel = m_TopElement.Q<Label>("char__power-label");
        }

        /// <summary>
        /// Register button callbacks for handling button click events.
        /// </summary>
        protected override void RegisterButtonCallbacks()
        {
            m_GearSlots[0].RegisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[1].RegisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[2].RegisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[3].RegisterCallback<ClickEvent>(ShowInventory);

            m_NextCharButton.RegisterCallback<ClickEvent>(GoToNextCharacter);
            m_LastCharButton.RegisterCallback<ClickEvent>(GoToLastCharacter);

            m_AutoEquipButton.RegisterCallback<ClickEvent>(AutoEquipSlots);
            m_UnequipButton.RegisterCallback<ClickEvent>(UnequipSlots);
            m_LevelUpButton.RegisterCallback<ClickEvent>(LevelUpCharacter);
        }

        /// <summary>
        /// Unregister button callbacks to prevent memory leaks. Optional in most cases,
        /// depending on application's lifecycle management.
        /// </summary>
        protected void UnregisterButtonCallbacks()
        {
            m_GearSlots[0].UnregisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[1].UnregisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[2].UnregisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[3].UnregisterCallback<ClickEvent>(ShowInventory);

            m_NextCharButton.UnregisterCallback<ClickEvent>(GoToNextCharacter);
            m_LastCharButton.UnregisterCallback<ClickEvent>(GoToLastCharacter);

            m_AutoEquipButton.UnregisterCallback<ClickEvent>(AutoEquipSlots);
            m_UnequipButton.UnregisterCallback<ClickEvent>(UnequipSlots);
            m_LevelUpButton.UnregisterCallback<ClickEvent>(LevelUpCharacter);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Show()
        {
            base.Show();

            MainMenuUIEvents.TabbedUIReset?.Invoke("CharScreen");
            CharEvents.ScreenStarted?.Invoke();
        }

        /// <summary>
        /// Hide the character view UI and notify end of screen.
        /// </summary>
        public override void Hide()
        {
            base.Hide();
            CharEvents.ScreenEnded?.Invoke();
        }

        /// <summary>
        /// Trigger level-up character functionality when the level-up button is clicked.
        /// </summary>
        /// <param name="evt"></param>
        void LevelUpCharacter(ClickEvent evt)
        {
            CharEvents.LevelUpClicked?.Invoke();
        }

        /// <summary>
        /// Notify CharScreenController to unequip all gear
        /// </summary>
        /// <param name="evt"></param>
        void UnequipSlots(ClickEvent evt)
        {
            AudioManager.PlayAltButtonSound();
            CharEvents.GearAllUnequipped?.Invoke();
        }

        /// <summary>
        /// Equip the best gear available in empty slots.
        /// </summary>
        void AutoEquipSlots(ClickEvent evt)
        {
            AudioManager.PlayAltButtonSound();
            CharEvents.GearAutoEquipped?.Invoke();
        }

        /// <summary>
        /// Select the previous character in the character view.
        /// </summary>
        void GoToLastCharacter(ClickEvent evt)
        {
            AudioManager.PlayAltButtonSound();
            CharEvents.LastCharacterSelected?.Invoke();
        }

        /// <summary>
        /// Select the next character in the character view.
        /// </summary>
        void GoToNextCharacter(ClickEvent evt)
        {
            AudioManager.PlayAltButtonSound();
            CharEvents.NextCharacterSelected?.Invoke();
        }

        /// <summary>
        /// Open the inventory screen when a gear slot is clicked.
        /// </summary>
        void ShowInventory(ClickEvent evt)
        {
            VisualElement clickedElement = evt.target as VisualElement;

            if (clickedElement == null)
                return;

            char slotNumber = clickedElement.name[clickedElement.name.Length - 1];
            int slot = (int)char.GetNumericValue(slotNumber) - 1;

            AudioManager.PlayDefaultButtonSound();

            MainMenuUIEvents.InventoryScreenShown?.Invoke();

            CharEvents.InventoryOpened?.Invoke(slot);
        }


        // Event-handling methods

        void OnInitialized()
        {
            SetVisualElements();
            RegisterButtonCallbacks();
        }

        /// <summary>
        /// Update the character view when a new character is selected.
        /// </summary>
        /// <param name="characterToShow">The character data to display.</param>
        void OnCharacterUpdated(CharacterData characterToShow)
        {
            if (characterToShow == null)
                return;

            // Update data sources for character labels
            m_CharacterLabel.dataSource = characterToShow;
            m_PowerLabel.dataSource = characterToShow;
            m_PotionsForNextLevel.dataSource = characterToShow;

            m_CharStatsView.UpdateCharacterStats(characterToShow);

            characterToShow.PreviewInstance.gameObject.SetActive(true);
        }

        /// <summary>
        /// Update the visual representation of a gear slot.
        /// </summary>
        /// <param name="gearData">The equipment data to display.</param>
        /// <param name="slotToUpdate">The gear slot index to update.</param>
        void OnGearSlotUpdated(EquipmentSO gearData, int slotToUpdate)
        {
            Button activeSlot = m_GearSlots[slotToUpdate];

            // plus symbol is the first child of char-inventory__slot-n
            VisualElement addSymbol = activeSlot.ElementAt(0);

            // background sprite is the second child of char-inventory__slot-n
            VisualElement gearElement = activeSlot.ElementAt(1);

            if (gearData == null)
            {
                if (gearElement != null)
                    gearElement.style.backgroundImage = new StyleBackground(m_EmptyGearSlotSprite);

                if (addSymbol != null)
                    addSymbol.style.display = DisplayStyle.Flex;
            }
            else
            {
                if (gearElement != null)
                    gearElement.style.backgroundImage = new StyleBackground(gearData.sprite);

                if (addSymbol != null)
                    addSymbol.style.display = DisplayStyle.None;
            }
        }

        /// <summary>
        /// Toggle level-up button VFX and state based on potion availability.
        /// </summary>
        /// <param name="state">True if level-up is possible, false otherwise.</param>
        void OnLevelUpButtonEnabled(bool state)
        {
            if (m_LevelUpButtonVFX == null || m_LevelUpButton == null)
                return;

            m_LevelUpButtonVFX.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;

            if (state)
            {
                // Enable the Button and allow the mouse pointer to activate the :hover pseudo-state
                m_LevelUpButton.SetEnabled(true);
                m_LevelUpButton.pickingMode = PickingMode.Position;

                // Add and remove the style classes to activate the Button
                m_LevelUpButton.AddToClassList(k_LevelUpButtonClass);
                m_LevelUpButton.RemoveFromClassList(k_LevelUpButtonInactiveClass);
            }
            else
            {
                // Disable the Button and don't allow the mouse pointer to activate the :hover pseudo-state
                m_LevelUpButton.SetEnabled(false);
                m_LevelUpButton.pickingMode = PickingMode.Ignore;
                m_LevelUpButton.AddToClassList(k_LevelUpButtonInactiveClass);
                m_LevelUpButton.RemoveFromClassList(k_LevelUpButtonClass);
            }
        }
    }
}
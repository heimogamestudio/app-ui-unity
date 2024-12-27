using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace UIToolkitDemo
{
    // Manages a single Gear Item UI component on the Inventory Screen
    public class GearItemComponent
    {
        EquipmentSO m_GearData;
        VisualElement m_Icon;
        VisualElement m_CheckMark;

        public EquipmentSO GearData => m_GearData;
        public VisualElement CheckMark => m_CheckMark;
        public VisualElement Icon => m_Icon;

        public GearItemComponent(EquipmentSO gearData)
        {
            m_GearData = gearData;
        }

        public void SetVisualElements(TemplateContainer gearElement)
        {
            if (gearElement == null)
                return;

            m_Icon = gearElement.Q("gear-item__icon");
            m_CheckMark = gearElement.Q("gear-item__checkmark");

            m_CheckMark.style.display = DisplayStyle.None;
        }

        public void SetGameData(TemplateContainer gearElement)
        {
            if (gearElement == null)
                return;

            m_Icon.style.backgroundImage = new StyleBackground(m_GearData.sprite);
        }

        public void RegisterButtonCallbacks()
        {
            m_Icon.RegisterCallback<ClickEvent>(ClickGearItem);
        }

        void ClickGearItem(ClickEvent evt)
        {
            ToggleCheckItem();

            // notify InventoryScreen this element is selected
            InventoryEvents.GearItemClicked?.Invoke(this);
        }

        public void CheckItem(bool state)
        {
            if (m_CheckMark == null)
                return;

            m_CheckMark.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        void ToggleCheckItem()
        {
            if (m_CheckMark == null)
                return;

            bool state = m_CheckMark.style.display == DisplayStyle.None;
            CheckItem(state);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UIToolkitDemo
{
    public class PotionData : MonoBehaviour
    {
        [SerializeField] uint m_MaxHealingPotions = 3;

        uint m_HealingPotionCount;

        public uint HealingPotionCount => m_HealingPotionCount;


        void OnEnable()
        {
            // listen for healed unit
            HealDropZone.UseOnePotion += OnUseOnePotion;
        }

        void OnDisable()
        {
            HealDropZone.UseOnePotion -= OnUseOnePotion;
        }

        void Start()
        {
            // start with max value and notify UI
            m_HealingPotionCount = m_MaxHealingPotions;

            GameplayEvents.HealingPotionUpdated?.Invoke((int)m_HealingPotionCount);
        }

        void UsePotion()
        {
            m_HealingPotionCount--;

            // notify the UI 
            GameplayEvents.HealingPotionUpdated?.Invoke((int) m_HealingPotionCount);

            AudioManager.PlayPotionDropSound();
        }

        // event-handling methods
        void OnUseOnePotion()
        {
            UsePotion();
        }
    }
}

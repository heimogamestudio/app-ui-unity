using System.Globalization;
using UnityEngine;
using Unity.Properties;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UIToolkitDemo
{
    public enum CharacterClass
    {
        Paladin,
        Wizard,
        Barbarian,
        Necromancer
    }

    public enum Rarity
    {
        Common,
        Rare,
        Special,
        All, // for filtering
    }

    public enum AttackType
    {
        Melee,
        Magic,
        Ranged
    }

    // baseline data for a specific character

    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Characters/CharacterGameData",
        menuName = "UIToolkitDemo/Character", order = 1)]
    public class CharacterBaseSO : ScriptableObject
    {
        [SerializeField] string m_CharacterName;

        [SerializeField] GameObject m_CharacterVisualsPrefab;

        [Header("Class attributes")]
        [SerializeField] CharacterClass m_CharacterClass;
        [SerializeField] LocalizedString m_CharacterClassLocalized;
        [SerializeField] Rarity m_Rarity;
        [SerializeField] LocalizedString m_RarityLocalized;
        [SerializeField] AttackType m_AttackType;
        [SerializeField] LocalizedString m_AttackTypeLocalized;
       
        [Header("Bio")]
        [SerializeField] string m_BioTitle;
        [SerializeField] LocalizedString m_BioTitleLocalized;

        [TextArea] [SerializeField] string m_Bio;

        [Header("Base points")]
        [SerializeField] float m_BasePointsLife;

        [SerializeField] float m_BasePointsDefense;

        [SerializeField] float m_BasePointsAttack;

        [SerializeField] float m_BasePointsAttackSpeed;

        [SerializeField] float m_BasePointsSpecialAttack;

        [SerializeField] float m_BasePointsCriticalHit;

        [Header("Skills")]
        [SerializeField] SkillSO m_Skill1;

        [SerializeField] SkillSO m_Skill2;

        [SerializeField] SkillSO m_Skill3;

        [Header("Default inventory gear")]
        // starting equipment (weapon, shield/armor, helmet, boots, gloves)
        [SerializeField] EquipmentSO m_DefaultWeapon;

        [SerializeField] EquipmentSO m_DefaultShieldAndArmor;

        [SerializeField] EquipmentSO m_DefaultHelmet;

        [SerializeField] EquipmentSO m_DefaultBoots;

        [SerializeField] EquipmentSO m_DefaultGloves;

        // Properties for data binding

        [CreateProperty] public LocalizedString CharacterClassLocalized => m_CharacterClassLocalized;

        [CreateProperty] public LocalizedString RarityLocalized => m_RarityLocalized;

        [CreateProperty] public LocalizedString AttackTypeLocalized => m_AttackTypeLocalized;

        [CreateProperty] public string CharacterName => m_CharacterName;

        [CreateProperty] public CharacterClass CharacterClass => m_CharacterClass;

        [CreateProperty] public Rarity Rarity => m_Rarity;

        [CreateProperty] public AttackType AttackType => m_AttackType;

        [CreateProperty] public string BioTitle => m_BioTitle;

        [CreateProperty] public LocalizedString BioTitleLocalized => m_BioTitleLocalized;

        [CreateProperty] public string Bio => m_Bio;

        [CreateProperty] public string BasePointsLife => m_BasePointsLife.ToString();

        [CreateProperty] public string BasePointsDefense => m_BasePointsDefense.ToString();

        [CreateProperty] public string BasePointsAttack => m_BasePointsAttack.ToString();

        [CreateProperty] public string BasePointsAttackSpeed => $"{m_BasePointsAttackSpeed:F1} /s";

        [CreateProperty] public string BasePointsSpecialAttack => $"{m_BasePointsSpecialAttack:F0} /s";

        [CreateProperty] public string BasePointsCriticalHit => m_BasePointsAttack.ToString();

        /// <summary>
        /// Used to calculate character "power" level.
        /// </summary>
        public float TotalBasePoints =>
            m_BasePointsAttack + m_BasePointsDefense + m_BasePointsLife + m_BasePointsCriticalHit;

        [CreateProperty] public SkillSO Skill1 => m_Skill1;
        [CreateProperty] public Sprite Skill1Icon => m_Skill1.IconSprite;

        [CreateProperty] public SkillSO Skill2 => m_Skill2;
        [CreateProperty] public Sprite Skill2Icon => m_Skill2.IconSprite;

        [CreateProperty] public SkillSO Skill3 => m_Skill3;
        [CreateProperty] public Sprite Skill3Icon => m_Skill3.IconSprite;

        [CreateProperty] public EquipmentSO DefaultWeapon => m_DefaultWeapon;

        [CreateProperty] public EquipmentSO DefaultShieldAndArmor => m_DefaultShieldAndArmor;

        [CreateProperty] public EquipmentSO DefaultHelmet => m_DefaultHelmet;

        [CreateProperty] public EquipmentSO DefaultBoots => m_DefaultBoots;

        [CreateProperty] public EquipmentSO DefaultGloves => m_DefaultGloves;

        [CreateProperty] public GameObject CharacterVisualsPrefab => m_CharacterVisualsPrefab;

        void OnValidate()
        {
            // Update localization keys when the enums change

            if (m_CharacterClassLocalized == null || m_CharacterClassLocalized.IsEmpty)
            {
                m_CharacterClassLocalized = new LocalizedString { TableReference = "SettingsTable" };
            }

            if (m_RarityLocalized == null || m_RarityLocalized.IsEmpty)
            {
                m_RarityLocalized = new LocalizedString { TableReference = "SettingsTable" };
            }

            if (m_AttackTypeLocalized == null || m_AttackTypeLocalized.IsEmpty)
            {
                m_AttackTypeLocalized = new LocalizedString { TableReference = "SettingsTable" };
            }

            
            // Always update the TableEntryReference to match current enum values
            m_CharacterClassLocalized.TableEntryReference = $"CharacterClass.{m_CharacterClass}";
            m_RarityLocalized.TableEntryReference = $"Rarity.{m_Rarity}";
            m_AttackTypeLocalized.TableEntryReference = $"AttackType.{m_AttackType}";
            

            if (m_DefaultWeapon != null && m_DefaultWeapon.equipmentType != EquipmentType.Weapon)
                m_DefaultWeapon = null;

            if (m_DefaultShieldAndArmor != null && m_DefaultShieldAndArmor.equipmentType != EquipmentType.Shield)
                m_DefaultShieldAndArmor = null;

            if (m_DefaultHelmet != null && m_DefaultHelmet.equipmentType != EquipmentType.Helmet)
                m_DefaultHelmet = null;

            if (m_DefaultGloves != null && m_DefaultGloves.equipmentType != EquipmentType.Gloves)
                m_DefaultGloves = null;

            if (m_DefaultBoots != null && m_DefaultBoots.equipmentType != EquipmentType.Boots)
                m_DefaultBoots = null;
        }
    }
}